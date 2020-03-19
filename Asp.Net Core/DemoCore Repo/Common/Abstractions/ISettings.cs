using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Abstractions
{
    public interface ISettings
    {
        string HomesSuiteHmsApi { get; }
        string PervasiveApi { get; }
        string SelectPurchaseOrderUserChange { get; }
        string GetPurchaseOrderLineById { get; }
        string GetPurchaseOrderLinesById { get; }
        string GetPurchaseOrderLines { get; }
        string GetPurchaseOrders { get; }
        string InsertPurchaseOrder { get; }
        string InsertPurchaseOrderLine { get; }
        string DeletePurchaseOrderHeader { get; }
        string DeletePurchaseOrderDetail { get; }
        string DeletePurchaseOrderDraftDetail { get; }
        string DeletePurchaseOrderDetails { get; }
        string UpdatePurchaseOrderDraftHeader { get; }
        string UpdatePurchaseOrderDraftDetail { get; }
        string GetPurchaseOrderById { get; }
        string GetPurchaseOrderChanges { get; }
        string GetPurchaseOrderChangeById { get; }
        string GetPurchaseOrderChangeHeaderById { get; }
        string InsertPurchaseOrderChangeHeader { get; }
        string DeletePurchaseOrderChangeHeader { get; }
        string GetPurchaseOrderChangeLinesById { get; }
        string GetPurchaseOrderChangeLineById { get; }
        string InsertPurchaseOrderChangeLine { get; }
        string DeletePurchaseOrderChangeLine { get; }
        string DeletePurchaseOrderChangeLines { get; }
        string GetPurchaseOrderReceiveHistory { get; }
        string GetApplicationDefaultDetails { get; }
        string ReadControlValue { get; }
        string GetVendorList { get; }
        string GetVendorById { get; }
        string ServiceUri { get; }
        string AdLoginUrl { get; }
        string ClientId { get; }
        string SecurityKey { get; }
        string ValidIssuer { get; }
        string ValidAudience { get; }
        int ExpireMinutes { get; }
        string GetShippingInfoList { get; }
        string GetShippingInfoByProfitCenter { get; }
        string GetUserClaims { get; }
        string GetUserDetailsById { get; }
        string GetUserDetailsByName { get; }
        string SearchItemMaster { get; }
        string GetItemMasterById { get; }
        string GetItemMasterList { get; }
        string GetResources { get; }
        string GetUsers { get; }
        string GetProfitCenters { get; }
        string GetProfitCenterById { get; }
        string GetRoles { get; }
        string UpdateUser { get; }
        string AddUserRole { get; }
        string DeleteUserRole { get; }
        string AddUserProfitCenter { get; }
        string DeleteUserProfitCenter { get; }

        string AllowedGroupIds { get; }
        string Authority { get; }
        string GraphResourceId { get; }
        string TenantId { get; }
        string GraphClientId { get; }
        string GraphSecret { get; }

        string InsertToken { get; }
        string GetTokenDetailsByToken { get; }
        string GetSalesOrder { get; }
        string GetSalesOrderById { get; }
        string GetSalesOrderLineByIds { get; }
        string GetNextSalesOrderLineNumber { get; }
        string GetSalesOrderHistorical { get; }
        string GetSalesOrderHistoricalById { get; }
        string SearchSalesOrders { get; }
        string SearchSalesOrdersDetail { get; }
        string AFIPurchaseOrderEntryUrl { get; }
        string AFIPurchaseOrderChangeUrl { get; }

        string GetInventoryItemList { get; }

        string GetTransferOrderLines { get; }

        int MemoryCacheExpireMinutes { get; }
        string GetPoDetailsByTripId { get; }
        string GetSalesOrderSettings { get; }
        string InsertWarehouseReceiveTransaction { get; }

        string InsertVendor { get; }
        string UpdateVendor { get; }

        string InsertPurchaseOrderUserChange { get; }
        string GetDefaultOption { get; }
        string GetLocations { get; }

        string UpdatePurchaseOrderChangeHeader { get; }

        string UpdatePurchaseOrderChangeLine { get; }
        string GetPurchaseOrderLinesByIdWithPendingChanges { get; }
        string SetDefaultOption { get; }
        string GetSalesOrderLinesByItemId { get; }
        string GetPurchaseOrderLineSummaryByItemId { get; }

        string GetPaymentsFromPervasive { get; }
        string GetNextPostingSequenceNumber { get; }
        string GetHomesUsers { get; }
        string GetHomesUserById { get; }
        string InsertHomesUser { get; }
        string UpdateHomesUser { get; }
        string GetUserExists { get; }
        string GetSalesPersonExists { get; }
        string GetNextUserId { get; }
    }
}
