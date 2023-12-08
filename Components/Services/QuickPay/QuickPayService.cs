using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Components.Services.Interfaces;
using Components.Services.QuickPay.Models;
using HtmlAgilityPack;
using Infrastructure.Cache;
using Infrastructure.DataModels;
using Infrastructure.Enums;
using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Components.Services.QuickPay
{
    public class QuickPayService : IQuickPayService
    {
        #region Private Members
        private readonly string _baseUrl;
        private readonly string _paymentUrl;
        private readonly string _acceptVersion;
        private readonly ILogging _logger;
        private readonly IOptions<AppSettings> _appSettings;
        #endregion

        #region Constructor
        public QuickPayService(IOptions<AppSettings> appSettings, ILogging logging)
        {
            _appSettings = appSettings;
            _baseUrl = _appSettings.Value.QuickPayBaseUrl;
            _paymentUrl = appSettings.Value.QuickPayPaymentUrl;
            _acceptVersion = _appSettings.Value.QuickPayAcceptVersion;
            _logger = logging;
        }
        #endregion

        #region Public Methods
        public PaymentOrders ParkingPayment(string amount, string month, string year, string cvv, string cardNumber)
        {
            PaymentOrders paymentOrder = null;
            PaymentModel paymentModel = this._GetInitialParkingPaymentModel("DKK");
            if (paymentModel.id != 0)
            {
                paymentOrder = this._GetPaymentOrder(paymentModel);
                PaymentAuthorize paymentAuthorize = this._GetParkingAuthorizeUrl(Convert.ToDecimal(amount), paymentModel.id);
                if (paymentAuthorize != null)
                {
                    string paidParkingSession = this._GetPaidParkingSession(paymentAuthorize.url);
                    if (!string.IsNullOrEmpty(paidParkingSession))
                    {
                        try
                        {
                            this._ProcessCard(paidParkingSession, month, year, cvv, cardNumber);
                        }
                        catch (Exception)
                        {
                            _logger.Debug(string.Format("Error occured while processing the card for payment: {0}", ErrorMessages.USER_CARD_INVALID.ToString()));
                            //throw new Park4YouException(ErrorMessages.USER_CARD_INVALID);
                        }
                        paymentOrder = this._GetPaymentDetails(paymentModel);
                    }
                }
            }
            return paymentOrder;
        }

        public void ValidateCard(string mobileNumber, string month, string year, string cvv, string cardNumber)
        {
            int cardId = this._CreateCard();
            string url = this._AuthorizeCard(cardId);
            if (!string.IsNullOrEmpty(url))
            {
                string sessionId = this._GetCardSession(url);
                if (!string.IsNullOrEmpty(sessionId))
                {
                    this._ProcessCard(sessionId, month, year, cvv, cardNumber);
                }
            }
        }
        #endregion

        #region Private Methods        
        private string _GetCardSession(string url)
        {
            _logger.Debug(string.Format("Going to get the card session from the url: {0}", url));
            string sessionId = string.Empty;
            using (WebClient webClient = new WebClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                string webResponse = webClient.DownloadString(url);
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(webResponse);
                var session = document.DocumentNode.SelectNodes(".//input[@name='session_id']").FirstOrDefault();
                if (session != null)
                    sessionId = session.Attributes["value"].Value;

                _logger.Debug(string.Format("SessionId is fetched and is: {0}", sessionId));

                return sessionId;
            }
        }

        private string _AuthorizeCard(int cardId)
        {
            _logger.Debug(string.Format("Going to authorize the card."));
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HttpRequestHeader.Authorization.ToString(), _GetToken());
                client.DefaultRequestHeaders.Add("Accept-Version", _acceptVersion);
                client.BaseAddress = new Uri(_baseUrl);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                var req = new HttpRequestMessage(HttpMethod.Put, "/cards/" + cardId + "/link");
                var res = client.SendAsync(req).Result;
                var content = res.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<SavedCardResponse>(content);
                _logger.Debug(string.Format("Card is authorize."));
                return data.url;
            }
        }

        private PaymentModel _GetInitialParkingPaymentModel(string currency)
        {
            _logger.Debug("Going to deposit the initial payment.");
            InitializePayment initializePayment = new InitializePayment()
            {
                currency = currency,
                order_id = CommonHelpers.GetOrderId()
            };
            using (WebClient webClient = new WebClient())
            {
                webClient.Headers.Clear();
                webClient.Headers.Add(HttpRequestHeader.Authorization, _GetToken());
                webClient.Headers.Add("Accept-Version", _acceptVersion);
                webClient.Headers.Add(HttpRequestHeader.Accept, Constants.Accept);
                webClient.Headers.Add(HttpRequestHeader.ContentType, Constants.ContentType);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                string response = webClient.UploadString(string.Concat(_baseUrl, "/payments"), JsonConvert.SerializeObject(initializePayment));
                PaymentModel rootObject = JsonConvert.DeserializeObject<PaymentModel>(response);
                _logger.Debug("Initial payment deposited.");
                return rootObject;
            }
        }

        private PaymentOrders _GetPaymentDetails(PaymentModel paymentModel)
        {
            _logger.Debug("Going to get the payment details.");
            PaymentOrders paymentOrders = null;
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Clear();
                    webClient.Headers.Add(HttpRequestHeader.Authorization, _GetToken());
                    webClient.Headers.Add("Accept-Version", _acceptVersion);
                    webClient.Headers.Add(HttpRequestHeader.Accept, Constants.Accept);
                    webClient.Headers.Add(HttpRequestHeader.ContentType, Constants.ContentType);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    string response = webClient.DownloadString(_baseUrl + "/payments/" + paymentModel.id);
                    PaymentModel updatedPaymentJson = JsonConvert.DeserializeObject<PaymentModel>(response);
                    if (updatedPaymentJson.operations != null && updatedPaymentJson.operations.Count > 0)
                    {
                        paymentOrders = this._GetPaymentOrder(updatedPaymentJson);
                    }
                }
            }
            catch (Exception)
            {
                _logger.Debug(string.Format("Exception occured at _GetPaymentDetails method"));
            }
            _logger.Debug("Payment details fetched.");
            return paymentOrders;
        }

        private PaymentAuthorize _GetParkingAuthorizeUrl(decimal amount, long transactionId)
        {
            _logger.Debug("Getting the authorize payment url.");
            TransactionAmount transactionAmount = new TransactionAmount()
            {
                amount = amount
            };
            using (WebClient webClient = new WebClient())
            {
                webClient.Headers.Clear();
                webClient.Headers.Add(HttpRequestHeader.Authorization, _GetToken());
                webClient.Headers.Add("Accept-Version", _acceptVersion);
                webClient.Headers.Add(HttpRequestHeader.Accept, Constants.Accept);
                webClient.Headers.Add(HttpRequestHeader.ContentType, Constants.ContentType);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                string response = webClient.UploadString(string.Concat(_baseUrl, "/payments/", transactionId, "/link"), HttpMethod.Put.ToString(), JsonConvert.SerializeObject(transactionAmount));
                PaymentAuthorize paymentAuthorize = JsonConvert.DeserializeObject<PaymentAuthorize>(response);
                _logger.Debug("Authorize payment url fetched.");
                return paymentAuthorize;
            }
        }

        private string _GetPaidParkingSession(string url)
        {
            _logger.Debug("Getting the session from the quickpay.");
            string sessionId = string.Empty;
            using (WebClient webClient = new WebClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                string webResponse = webClient.DownloadString(url);
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(webResponse);
                var session = document.DocumentNode.SelectNodes(".//input[@name='session_id']").FirstOrDefault();
                if (session != null)
                    sessionId = session.Attributes["value"].Value;

                _logger.Debug("Session fetched from the quickpay.");

                return sessionId;
            }
        }

        private void _ProcessCard(string session_id, string expirationMonth, string expirationYear, string cvv, string cardNumber)
        {
            _logger.Debug("Going to process the card for the payment.");
            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("session_id", session_id));
            nvc.Add(new KeyValuePair<string, string>("expiration[year]", expirationYear));
            nvc.Add(new KeyValuePair<string, string>("expiration[month]", expirationMonth));
            nvc.Add(new KeyValuePair<string, string>("cvd", cvv));
            nvc.Add(new KeyValuePair<string, string>("cardnumber", cardNumber));
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HttpRequestHeader.Authorization.ToString(), _GetToken());
                client.DefaultRequestHeaders.Add("Accept-Version", _acceptVersion);
                client.BaseAddress = new Uri(_paymentUrl);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                var req = new HttpRequestMessage(HttpMethod.Post, "/process_card") { Content = new FormUrlEncodedContent(nvc) };
                var res = client.SendAsync(req).Result;
                var content = res.Content.ReadAsStringAsync().Result;
                if (res.StatusCode != HttpStatusCode.OK)
                    throw new Park4YouException(ErrorMessages.USER_CARD_INVALID);
            }
            _logger.Debug("Processed card for the payment has been successfully done.");
        }

        private int _CreateCard()
        {
            _logger.Debug(string.Format("Going to create the card."));
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HttpRequestHeader.Authorization.ToString(), _GetToken());
                client.DefaultRequestHeaders.Add("Accept-Version", _acceptVersion);
                client.BaseAddress = new Uri(_baseUrl);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                var req = new HttpRequestMessage(HttpMethod.Post, "/cards");
                var res = client.SendAsync(req).Result;
                var content = res.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<CardModel>(content);
                _logger.Debug(string.Format("Card is successfully created."));
                return data.id;
            }
        }

        private string _GetToken()
        {
            string password = _appSettings.Value.QuickPayPassword;
            string username = string.Empty;
            string authToken = Convert.ToBase64String(Encoding.Default.GetBytes(string.Concat(username, ":", password)));
            authToken = string.Concat(Constants.BasicAuthentication, authToken);
            return authToken;
        }

        private PaymentOrders _GetPaymentOrder(PaymentModel paymentModel)
        {
            PaymentOrders paymentOrder = new PaymentOrders();

            paymentOrder.ParkingId = 0;
            paymentOrder.PaymentOrderId = paymentModel.id;
            paymentOrder.MerchantId = paymentModel.merchant_id;
            paymentOrder.OrderId = paymentModel.order_id;
            paymentOrder.Accepted = paymentModel.accepted == null ? false : (bool)paymentModel.accepted;
            paymentOrder.OrderType = paymentModel.type;
            paymentOrder.TextOnStatement = paymentModel.text_on_statement;
            paymentOrder.BrandingId = paymentModel.branding_id;
            paymentOrder.Currency = paymentModel.currency;
            paymentOrder.State = paymentModel.state;
            if (paymentModel.metadata != null)
            {
                paymentOrder.MetaDataType = paymentModel.metadata.type;
                paymentOrder.Origin = paymentModel.metadata.origin;
                paymentOrder.Brand = paymentModel.metadata.brand;
                paymentOrder.Bin = paymentModel.metadata.bin;
                paymentOrder.Last4 = paymentModel.metadata.last4;
                paymentOrder.ExpMonth = paymentModel.metadata.exp_month;
                paymentOrder.ExpYear = paymentModel.metadata.exp_year;
                paymentOrder.Country = paymentModel.metadata.country;
                paymentOrder.Is3dSecure = paymentModel.metadata.is_3d_secure;
                paymentOrder.IssuedTo = paymentModel.metadata.issued_to;
                paymentOrder.Hash = paymentModel.metadata.hash;
                paymentOrder.Number = paymentModel.metadata.number;
                paymentOrder.CustomerIP = paymentModel.metadata.customer_ip;
                paymentOrder.CustomerCountry = paymentModel.metadata.customer_country;
                paymentOrder.FraudSuspected = paymentModel.metadata.fraud_suspected;
                paymentOrder.NinCountryCode = paymentModel.metadata.nin_country_code;
                paymentOrder.NinGender = paymentModel.metadata.nin_gender;
            }

            if (paymentModel.link != null)
            {
                paymentOrder.URL = paymentModel.link.url;
                paymentOrder.AgreementId = paymentModel.link.agreement_id;
                paymentOrder.Language = paymentModel.link.language;
                paymentOrder.Amount = paymentModel.link.amount;
                paymentOrder.ContinueURL = paymentModel.link.continue_url;
                paymentOrder.CancelURL = paymentModel.link.cancel_url;
                paymentOrder.CallbackURL = paymentModel.link.callback_url;
                paymentOrder.PaymentMethods = paymentModel.link.payment_methods;
                paymentOrder.AutoFee = paymentModel.link.auto_fee;
                paymentOrder.AutoCapture = paymentModel.link.auto_capture;
                paymentOrder.LinkBrandingId = paymentModel.link.branding_id;
                paymentOrder.Version = paymentModel.link.version;
            }

            if (paymentModel.operations != null && paymentModel.operations.Count > 0)
            {
                paymentOrder.OperationId = paymentModel.operations[0].id;
                paymentOrder.OperationType = paymentModel.operations[0].type;
                paymentOrder.OperationAmount = paymentModel.operations[0].amount.ToString();
                paymentOrder.Pending = paymentModel.operations[0].pending;
                paymentOrder.QPStatusCode = paymentModel.operations[0].qp_status_code;
                paymentOrder.QPStatusMessage = paymentModel.operations[0].qp_status_msg;
                paymentOrder.OperationCreatedDate = paymentModel.operations[0].created_at;
            }

            paymentOrder.TestMode = paymentModel.test_mode;
            paymentOrder.Acquirer = paymentModel.acquirer;
            paymentOrder.CreatedAt = paymentModel.created_at;
            paymentOrder.UpdatedAt = paymentModel.updated_at;
            paymentOrder.ReturnedAt = paymentModel.retented_at;
            paymentOrder.Balance = paymentModel.balance;
            paymentOrder.Fee = paymentModel.fee;
            paymentOrder.SystemCreatedDate = DateTime.UtcNow;

            return paymentOrder;
        }
        #endregion
    }
}