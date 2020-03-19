using Common.Abstractions.Configuration;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Framework.Builders
{
    public class AppSettings : IAppSettings
    {
        public AppSettings()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", false)
                .Build();

            AshleyDbConnection = configuration.GetConnectionString("AshleyDbConnection");
            AshleyHsDbConnection = configuration.GetConnectionString("AshleyHsDbConnection");
            AuditDbConnection = configuration.GetConnectionString("AuditDbConnection");
            HomesSuiteHmsApi = configuration.GetSection("HomesSuiteHmsApi").Value;
            PervasiveApi = configuration.GetSection("PervasiveApi").Value;
            SelectPurchaseOrderUserChange = configuration["StoredProcedures:SelectPurchaseOrderUserChange"];
            GetPurchaseOrderLineById = configuration["StoredProcedures:GetPurchaseOrderLineById"];
            GetPurchaseOrderLinesById = configuration["StoredProcedures:GetPurchaseOrderLinesById"];
            GetPurchaseOrderLines = configuration["StoredProcedures:GetPurchaseOrderLines"];
            GetPurchaseOrders = configuration["StoredProcedures:GetPurchaseOrders"];
            InsertPurchaseOrder = configuration["StoredProcedures:InsertPurchaseOrder"];
            InsertPurchaseOrderLine = configuration["StoredProcedures:InsertPurchaseOrderLine"];
            DeletePurchaseOrderHeader = configuration["StoredProcedures:DeletePurchaseOrderHeader"];
            DeletePurchaseOrderDetail = configuration["StoredProcedures:DeletePurchaseOrderDetail"];
            DeletePurchaseOrderDraftDetail = configuration["StoredProcedures:DeletePurchaseOrderDraftDetail"];
            DeletePurchaseOrderDetails = configuration["StoredProcedures:DeletePurchaseOrderDetails"];
            UpdatePurchaseOrderDraftHeader = configuration["StoredProcedures:UpdatePurchaseOrderDraftHeader"];
            UpdatePurchaseOrderDraftDetail = configuration["StoredProcedures:UpdatePurchaseOrderDraftDetail"];
            GetPurchaseOrderById = configuration["StoredProcedures:GetPurchaseOrderById"];
            GetPurchaseOrderChanges = configuration["StoredProcedures:GetPurchaseOrderChanges"];
            GetPurchaseOrderChangeById = configuration["StoredProcedures:GetPurchaseOrderChangeById"];
            GetPurchaseOrderChangeHeaderById = configuration["StoredProcedures:GetPurchaseOrderChangeHeaderById"];
            InsertPurchaseOrderChangeHeader = configuration["StoredProcedures:InsertPurchaseOrderChangeHeader"];
            DeletePurchaseOrderChangeHeader = configuration["StoredProcedures:DeletePurchaseOrderChangeHeader"];
            GetPurchaseOrderChangeLineById = configuration["StoredProcedures:GetPurchaseOrderChangeLineById"];
            GetPurchaseOrderChangeLinesById = configuration["StoredProcedures:GetPurchaseOrderChangeLinesById"];
            InsertPurchaseOrderChangeLine = configuration["StoredProcedures:InsertPurchaseOrderChangeLine"];
            DeletePurchaseOrderChangeLine = configuration["StoredProcedures:DeletePurchaseOrderChangeLine"];
            DeletePurchaseOrderChangeLines = configuration["StoredProcedures:DeletePurchaseOrderChangeLines"];
            GetPurchaseOrderReceiveHistory = configuration["StoredProcedures:GetPurchaseOrderReceiveHistory"];
            GetApplicationDefaultDetails = configuration["StoredProcedures:GetApplicationDefaultDetails"];
            ReadControlValue = configuration["StoredProcedures:ReadControlValue"];
            GetVendorList = configuration["StoredProcedures:GetVendorList"];
            GetVendorById = configuration["StoredProcedures:GetVendorById"];
            GetUserClaims = configuration["StoredProcedures:GetUserClaims"];
            GetUserDetailsById = configuration["StoredProcedures:GetUserDetailsById"];
            GetUserDetailsByName = configuration["StoredProcedures:GetUserDetailsByName"];
            ServiceUri = configuration["Auth:ServiceUri"];
            AdLoginUrl = configuration["Auth:AdLoginUrl"];
            ClientId = configuration["Auth:ClientId"];
            SecurityKey = configuration["Auth:SecurityKey"];
            ValidIssuer = configuration["Auth:ValidIssuer"];
            ValidAudience = configuration["Auth:ValidAudience"];
            ExpireMinutes = int.Parse(configuration["Auth:ExpireMinutes"]);
            GetShippingInfoList = configuration["StoredProcedures:GetShippingInfoList"];
            GetShippingInfoByProfitCenter = configuration["StoredProcedures:GetShippingInfoByProfitCenter"];
            SearchItemMaster = configuration["StoredProcedures:SearchItemMaster"];
            GetItemMasterById = configuration["StoredProcedures:GetItemMasterById"];
            GetItemMasterList = configuration["StoredProcedures:GetItemMasterList"];
            GetResources = configuration["StoredProcedures:GetResources"];
            InsertToken = configuration["StoredProcedures:InsertToken"];
            GetTokenDetailsByToken = configuration["StoredProcedures:GetTokenDetailsByToken"];
            GetResources = configuration["StoredProcedures:GetResources"];
            AllowedGroupIds = configuration["Auth:GraphClient:AllowedGroupIds"];
            Authority = configuration["Auth:GraphClient:Authority"];
            GraphResourceId = configuration["Auth:GraphClient:GraphResourceId"];
            TenantId = configuration["Auth:GraphClient:TenantId"];
            GraphClientId = configuration["Auth:GraphClient:GraphClientId"];
            GraphSecret = configuration["Auth:GraphClient:GraphSecret"];
            GetResources = configuration["StoredProcedures:GetResources"];
            GetUsers = configuration["StoredProcedures:GetUsers"];
            GetProfitCenters = configuration["StoredProcedures:GetProfitCenters"];
            GetProfitCenterById = configuration["StoredProcedures:GetProfitCenterById"];
            GetRoles = configuration["StoredProcedures:GetRoles"];
            UpdateUser = configuration["StoredProcedures:UpdateUser"];
            AddUserRole = configuration["StoredProcedures:AddUserRole"];
            DeleteUserRole = configuration["StoredProcedures:DeleteUserRole"];
            AddUserProfitCenter = configuration["StoredProcedures:AddUserProfitCenter"];
            DeleteUserProfitCenter = configuration["StoredProcedures:DeleteUserProfitCenter"];
            GetInventoryItemList = configuration["StoredProcedures:GetInventoryItemList"];
            GetTransferOrderLines = configuration["StoredProcedures:GetTransferOrderLines"];
            HomesDbConnection = configuration.GetConnectionString("HomesDbConnection");
            MemoryCacheExpireMinutes = int.Parse(configuration["MemoryCache:ExpireMinutes"]);
            GetSalesOrder = configuration["MemoryCache:GetSalesOrder"];
            GetSalesOrderById = configuration["MemoryCache:GetSalesOrderById"];
            GetSalesOrderLineByIds = configuration["MemoryCache:GetSalesOrderLineByIds"];
            GetNextSalesOrderLineNumber = configuration["MemoryCache:GetNextSalesOrderLineNumber"];
            GetSalesOrderHistorical = configuration["MemoryCache:GetSalesOrderHistorical"];
            GetSalesOrderHistoricalById = configuration["MemoryCache:GetSalesOrderHistoricalById"];
            GetResources = configuration["StoredProcedures:GetResources"];
            GetResources = configuration["StoredProcedures:GetResources"];
            GetPoDetailsByTripId = configuration["StoredProcedures:GetPoDetailsByTripId"];
            GetSalesOrderSettings = configuration["StoredProcedures:GetSalesOrderSettings"];
            SearchSalesOrders = configuration["StoredProcedures:SearchSalesOrders"];
            SearchSalesOrdersDetail = configuration["StoredProcedures:SearchSalesOrdersDetail"];
            InsertVendor = configuration["StoredProcedures:InsertVendor"];
            UpdateVendor = configuration["StoredProcedures:UpdateVendor"];
            InsertWarehouseReceiveTransaction = configuration["StoredProcedures:InsertWarehouseReceiveTransaction"];
            InsertPurchaseOrderUserChange = configuration["StoredProcedures:InsertPurchaseOrderUserChange"];
            GetDefaultOption = configuration["StoredProcedures:GetDefaultOption"];
            GetLocations = configuration["StoredProcedures:GetLocations"];
            GetDefaultOption = configuration["StoredProcedures:GetDefaultOption"];
            GetPurchaseOrderLinesByIdWithPendingChanges = configuration["StoredProcedures:GetPurchaseOrderLinesByIdWithPendingChanges"];
            SetDefaultOption = configuration["StoredProcedures:SetDefaultOption"];
            GetSalesOrderLinesByItemId = configuration["StoredProcedures:GetSalesOrderLinesByItemId"];
            GetPurchaseOrderLineSummaryByItemId = configuration["StoredProcedures:GetPurchaseOrderLineSummaryByItemId"];
            GetHomesUsers = configuration["StoredProcedures:GetHomesUsers"];
            GetHomesUserById = configuration["StoredProcedures:GetHomesUserById"];
            InsertHomesUser = configuration["StoredProcedures:InsertHomesUser"];
            UpdateHomesUser = configuration["StoredProcedures:UpdateHomesUser"];
            GetUserExists = configuration["StoredProcedures:GetUserExists"];
            GetSalesPersonExists = configuration["StoredProcedures:GetSalesPersonExists"];
            GetNextUserId = configuration["StoredProcedures:GetNextUserId"];
            AFIPurchaseOrderEntryUrl = configuration.GetSection("AFIPOEntryUrl").Value;
            AFIPurchaseOrderChangeUrl = configuration.GetSection("AFIPOChangeUrl").Value;
            UpdatePurchaseOrderChangeHeader = configuration["StoredProcedures:UpdatePurchaseOrderChangeHeader"];
            UpdatePurchaseOrderChangeLine = configuration["StoredProcedures:UpdatePurchaseOrderChangeLine"];
            GetPaymentsFromPervasive = configuration["StoredProcedures:GetPaymentsFromPervasive"];
            GetNextPostingSequenceNumber = configuration["StoredProcedures:GetNextPostingSequenceNumber"];
        }

        public string AshleyDbConnection { get; }
        public string AshleyHsDbConnection { get; }
        public string AuditDbConnection { get; }
        public string HomesDbConnection { get; }
        public string HomesSuiteHmsApi { get; }
        public string PervasiveApi { get; }
        public string SelectPurchaseOrderUserChange { get; }
        public string GetPurchaseOrderLineById { get; }
        public string GetPurchaseOrderLinesById { get; }
        public string GetPurchaseOrderLines { get; }
        public string GetPurchaseOrders { get; }
        public string InsertPurchaseOrder { get; }
        public string InsertPurchaseOrderLine { get; }
        public string DeletePurchaseOrderHeader { get; }
        public string DeletePurchaseOrderDetail { get; }
        public string DeletePurchaseOrderDraftDetail { get; }
        public string DeletePurchaseOrderDetails { get; }
        public string UpdatePurchaseOrderDraftHeader { get; }
        public string UpdatePurchaseOrderDraftDetail { get; }
        public string GetPurchaseOrderById { get; }
        public string GetPurchaseOrderChanges { get; }
        public string GetPurchaseOrderChangeById { get; }
        public string GetPurchaseOrderChangeHeaderById { get; }
        public string InsertPurchaseOrderChangeHeader { get; }
        public string DeletePurchaseOrderChangeHeader { get; }
        public string GetPurchaseOrderChangeLineById { get; }
        public string GetPurchaseOrderChangeLinesById { get; }
        public string InsertPurchaseOrderChangeLine { get; }
        public string DeletePurchaseOrderChangeLine { get; }
        public string DeletePurchaseOrderChangeLines { get; }
        public string GetPurchaseOrderReceiveHistory { get; }
        public string GetApplicationDefaultDetails { get; }
        public string ReadControlValue { get; }
        public string GetVendorList { get; }
        public string GetVendorById { get; }
        public string GetUserClaims { get; }
        public string GetUserDetailsById { get; }
        public string GetUserDetailsByName { get; }
        public string GetShippingInfoList { get; }
        public string GetShippingInfoByProfitCenter { get; }
        public string GetResources { get; }
        public string SecurityKey { get; }
        public string ValidIssuer { get; }
        public string ValidAudience { get; }
        public int ExpireMinutes { get; }
        public string ServiceUri { get; }
        public string AdLoginUrl { get; }
        public string ClientId { get; }
        public string SearchItemMaster { get; }
        public string GetItemMasterById { get; }
        public string GetItemMasterList { get; }
        public string GetSalesOrder { get; }
        public string GetSalesOrderById { get; }
        public string GetSalesOrderLineByIds { get; }
        public string GetNextSalesOrderLineNumber { get; }
        public string GetSalesOrderHistorical { get; }
        public string GetSalesOrderHistoricalById { get; }
        public string SearchSalesOrders { get; }
        public string SearchSalesOrdersDetail { get; }
        public string AllowedGroupIds { get; }
        public string Authority { get; }
        public string GraphResourceId { get; }
        public string TenantId { get; }
        public string GraphClientId { get; }
        public string GraphSecret { get; }
        public string InsertToken { get; }
        public string GetTokenDetailsByToken { get; }
        public string AFIPurchaseOrderEntryUrl { get; }
        public string AFIPurchaseOrderChangeUrl { get; }
        public string GetUsers { get; }
        public string GetProfitCenters { get; }
        public string GetProfitCenterById { get; }
        public string GetRoles { get; }
        public string UpdateUser { get; }
        public string AddUserRole { get; }
        public string DeleteUserRole { get; }
        public string AddUserProfitCenter { get; }
        public string DeleteUserProfitCenter { get; }

        public string GetInventoryItemList { get; }

        public string GetTransferOrderLines { get; }

        public int MemoryCacheExpireMinutes { get; }

        public string GetPoDetailsByTripId { get; }
        public string GetSalesOrderSettings { get; }

        public string InsertVendor { get; }
        public string UpdateVendor { get; }
        public string InsertWarehouseReceiveTransaction { get; }

        public string InsertPurchaseOrderUserChange { get; }
        public string GetDefaultOption { get; }
        public string GetLocations { get; }

        public string UpdatePurchaseOrderChangeHeader { get; }
        public string UpdatePurchaseOrderChangeLine { get; }

        public string GetPurchaseOrderLinesByIdWithPendingChanges { get; }
        public string SetDefaultOption { get; }
        public string GetSalesOrderLinesByItemId { get; }
        public string GetPurchaseOrderLineSummaryByItemId { get; }

        public string GetPaymentsFromPervasive { get; }
        public string GetNextPostingSequenceNumber { get; }
        public string GetHomesUsers { get; }
        public string GetHomesUserById { get; }

        public string InsertHomesUser { get; }
        public string UpdateHomesUser { get; }
        public string GetUserExists { get; }
        public string GetSalesPersonExists { get; }
        public string GetNextUserId { get; }
    }
}
