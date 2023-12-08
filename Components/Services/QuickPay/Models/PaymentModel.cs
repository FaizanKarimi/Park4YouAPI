using System;
using System.Collections.Generic;

namespace Components.Services.QuickPay.Models
{
    public class PaymentModel
    {
        public long id { get; set; }
        public long merchant_id { get; set; }
        public string order_id { get; set; }
        public bool? accepted { get; set; }
        public string type { get; set; }
        public string text_on_statement { get; set; }
        public string branding_id { get; set; }
        public string currency { get; set; }
        public string state { get; set; }
        public MetaDataModel metadata { get; set; }
        public LinkModel link { get; set; }
        public string shipping_address { get; set; }
        public string invoice_address { get; set; }
        public List<OperationModel> operations { get; set; }
        public string test_mode { get; set; }
        public string acquirer { get; set; }
        public string facilitator { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? retented_at { get; set; }
        public int balance { get; set; }
        public string fee { get; set; }
        public DateTime? deadline_at { get; set; }
    }

    public class OperationModel
    {
        public int? id { get; set; }
        public string type { get; set; }
        public int? amount { get; set; }
        public bool? pending { get; set; }
        public string qp_status_code { get; set; }
        public string qp_status_msg { get; set; }
        public string aq_status_code { get; set; }
        public string aq_status_msg { get; set; }
        public string callback_url { get; set; }
        public string callback_success { get; set; }
        public string callback_response_code { get; set; }
        public string callback_duration { get; set; }
        public string acquirer { get; set; }
        public string callback_at { get; set; }
        public DateTime? created_at { get; set; }
    }

    public class LinkModel
    {
        public string url { get; set; }
        public int? agreement_id { get; set; }
        public string language { get; set; }
        public int? amount { get; set; }
        public string continue_url { get; set; }
        public string cancel_url { get; set; }
        public string callback_url { get; set; }
        public string payment_methods { get; set; }
        public string auto_fee { get; set; }
        public string auto_capture { get; set; }
        public string branding_id { get; set; }
        public string google_analytics_client_id { get; set; }
        public string google_analytics_tracking_id { get; set; }
        public string version { get; set; }
        public string acquirer { get; set; }
        public string deadline { get; set; }
        public bool? framed { get; set; }
        public string customer_email { get; set; }
    }

    public class MetaDataModel
    {
        public string type { get; set; }
        public string origin { get; set; }
        public string brand { get; set; }
        public string bin { get; set; }
        public string last4 { get; set; }
        public string exp_month { get; set; }
        public string exp_year { get; set; }
        public string country { get; set; }
        public bool? is_3d_secure { get; set; }
        public string issued_to { get; set; }
        public string hash { get; set; }
        public string number { get; set; }
        public string customer_ip { get; set; }
        public string customer_country { get; set; }
        public bool? fraud_suspected { get; set; }
        public string nin_country_code { get; set; }
        public string nin_gender { get; set; }
    }
}