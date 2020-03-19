using Common.Abstractions;
using Common.Abstractions.Configuration;
using Common.Abstractions.Data;
using Common.Exceptions;
using Common.Models;
using Dapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class HomesUserRepository : AsyncDbRepository, IHomesUserRepository
    {

        private readonly IAppSettings _configuration;
        private ILogger<HomesUserRepository> _logger;
        private IApplicationDefaultsRepository _applicationDefaultsRepository;
        private IHomesUserSqlRepository _homesUserSqlRepository;
        private ApplicationDefault applicationDefault;
        private bool usingProfit = false;
        private bool usingPervasive = true;
        const string contentType = "application/json";
        private const string DefaultDescription = "HMS_Using_Profit";
        private const string DefaultDescriptionUsingPervasive = "HMS_Using_Pervasive";
        private const string DefaultPC = "GLOBAL";

        public HomesUserRepository(IAppSettings configuration, IAshleyDbConnectionFactory connectionFactory, ILogger<HomesUserRepository> logger, IHomesUserSqlRepository homesUserSqlRepository, IApplicationDefaultsRepository applicationDefaultsRepository) : base(connectionFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _applicationDefaultsRepository = applicationDefaultsRepository;
            _homesUserSqlRepository = homesUserSqlRepository;
            applicationDefault = _applicationDefaultsRepository.GetAsync(DefaultDescription, DefaultPC).Result;
            usingProfit = Convert.ToBoolean(applicationDefault.default_Value);
            applicationDefault = _applicationDefaultsRepository.GetAsync(DefaultDescriptionUsingPervasive, DefaultPC).Result;
            usingPervasive = Convert.ToBoolean(applicationDefault.default_Value);
            if (connectionFactory == null) throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<HomesUser> AddAsync(HomesUser homesUser)
        {
            HttpResponseMessage response = new HttpResponseMessage();

           PervasiveDataInfo pervasiveDataInfo = new PervasiveDataInfo();
            pervasiveDataInfo.DynamicParameters = new Dictionary<string, object>();

            HttpClient PervasiveApiClient = new HttpClient();

            var queryParameters = new DynamicParameters();
            queryParameters.Add("@UserName", homesUser.UserName);

            var userExists = (WithConnection(async c => await c.QueryAsync<dynamic>(sql: _configuration.GetUserExists, param: queryParameters, commandType: CommandType.StoredProcedure)).Result);
            var intUserExists = userExists.FirstOrDefault() as IDictionary<string, object>;

            if (Convert.ToInt32(intUserExists.Values.FirstOrDefault()) > 0)
            {
                HomesUserException exception = new HomesUserException("User already exists.");
                throw exception;
            }


            var nextUserId = (WithConnection(async c => await c.QueryAsync<dynamic>(sql: _configuration.GetNextUserId, commandType: CommandType.StoredProcedure)).Result);
            homesUser.UserId = Convert.ToInt32((nextUserId.FirstOrDefault() as IDictionary<string, object>).Values.FirstOrDefault());

            pervasiveDataInfo.PervasiveSQL = "INSERT INTO usr(usr_id, usr_nam, usr_pass, usr_pft_ctr, usr_spins, usr_sec_lvl, usr_slm, usr_scrn_res_flg, usr_welcome_scrn, usr_use_drw_flg, usr_drw, usr_beg_balance, usr_coa_budgn_comps, usr_st_prt_aging, usr_st_prt_zero_bal, usr_st_prt_cr_bal, usr_st_prt_avl_cr, usr_st_prt_ins_info, usr_st_run_day, usr_st_cycs_to_proc, usr_st_min_bil, usr_st_prt_comp_nam, usr_st_even_dol_pmt, usr_age_dtl_summ, usr_age_use_his_fil, usr_age_day, usr_age_anal_st_day, usr_age_itmspst_dat, usr_age_prtpr_dtl, usr_agecutof_baldue, usr_agecutof_prd, usr_age_only_cr_bal, usr_age_prtzero_bal, usr_ageignr_rsv_pmt, usr_age_prt_cr_lim, usr_age_prtanl_only, usr_age_prt_rec_age, usr_age_prt_addr, usr_age_prt_remarks, usr_age_seq, usr_pt_first_prc, usr_pt_1st_prc_lit, usr_pt_encr_1st_prc, usr_pt_2nd_prc, usr_pt_2nd_prc_lit, usr_pt_encr_2nd_prc, usr_pt_3rd_prc, usr_pt_3rd_prc_lit, usr_pt_encr_3rd_prc, usr_pt_cost, usr_pt_cost_lit, usr_pt_encr_cost, usr_pt_bldg_to_prt, usr_pt_lbls_across, usr_pt_qty_to_prt, usr_pt_qty_ohor_loc, usr_pt_prt_aux_desc, usr_pt_prt_comp_nam, usr_ss_alpha_or_num, usr_ss_cost_or_loc, usr_ss_dtl_or_sum, usr_ss_skipzerox_bo, usr_ss_skipzeron_bo, usr_ss_skipzerob_bo, usr_ss_prt_prc_1, usr_ss_prt_prc_2, usr_ss_prt_prc_3, usr_ss_prtonlyw_loc, usr_ss_prtavg_w_loc, usr_ss_pag_break, usr_invcprt_compnam, usr_invcprt_pkg_rec, usr_invcprt_pkg_prc, usr_prt_pkg_item, usr_prtpkg_item_prc, usr_prt_comp_adrs, usr_dr_prt_pkg_rec, usr_dr_prt_pkg_prc, usr_dr_prt_item_prc, usr_crprt_rcpt_dflt, usr_cr_auto_tear, usr_crprt_rcpt_item, usr_cr_prt_rcpt_slm, usr_cr_sub_tot, usr_poprt_form_cost, usr_po_prt_prt_cost, usr_po_prt_seq, usr_po_prt_comp_nam, usr_poprt_comp_adrs, usr_poprt_slsperson, usr_po_prt_cust_inf, usr_so_pak_list_flg, usr_soauto_upd_cust, usr_so_prt_ins_form, usr_soprt_cntr_form, usr_so_prt_so, usr_so_prt_del_rcp, usr_soreserve_to_po, usr_so_auto_gen_po, usr_so_man_assign, usr_so_auto_assign, usr_so_dp_dflt, usr_delrcp, usr_sobo_on_shp_lbl, usr_so_del_rcp_tax, usr_so_del_rcp_ul, usr_so_prt_desc_1, usr_del_rcp_abbrev, usr_pkg_eq_items, usr_item_remarks, usr_coll_bookmarks, usr_coll_view, usr_bc_printer, usr_bc_prc_flg, usr_tb_buttons, usr_lsr_ar_form_flg, usr_new_so_form_flg, usr_so_lf_sect1, usr_so_lf_sect2, usr_so_lf_sect3, usr_so_lf_sect4, usr_po_lf_sect1, usr_po_lf_sect2, usr_po_lf_sect3, usr_ap_lf_sect1, usr_ap_lf_sect2, usr_ap_lf_sect3, usr_ap_lf_sect4, usr_ar_lf_sect1, usr_ar_lf_sect2, usr_ar_lf_sect3, usr_ar_lf_sect4, usr_seq_cr_appr, usr_seq_csh, usr_seq_csh_drw, usr_seq_mrktg, usr_seq_billg, usr_seq_tax, usr_seq_bill2, usr_seq_comm, usr_seq_del2, usr_seq_delivery, usr_seq_terms, usr_seq_totals, usr_seq_so_dtl, usr_seq_pft_ctr, usr_seq_prt, usr_seq_sale_dat, usr_seq_slm_1, usr_seq_del_dat, usr_seq_so_addl_rem, usr_seq_typ, usr_seq_xtra_2, usr_seq_xtra_3, usr_seq_xtra_4, usr_seq_xtra_5, usr_seq_xtra_6, usr_seq_xtra_7, usr_seq_xtra_8, usr_seq_xtra_9, usr_seq_xtra_10, usr_ascii_xml, usr_ap_dist_popup, usr_asap_dflt, usr_serial_on_form, usr_ask_prev_po, usr_prt_ap_rem, usr_prt_dat_n_disc, usr_prt_chk_no, usr_lsr_ap_chk, usr_v_studio, usr_prt_so_curr_dat, usr_so_prt_opt, usr_so_prt_so_rem, usr_bc_printer_typ, usr_so_prt_tax_rt, usr_so_prt_fabric, usr_so_prt_frame, usr_so_prt_finish, usr_so_prt_trim, usr_so_prt_other, usr_filler_1, usr_filler_2) " +
            "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

            pervasiveDataInfo.DynamicParameters.Add("usr_id", homesUser.UserId);
            pervasiveDataInfo.DynamicParameters.Add("usr_nam", homesUser.UserName);
            pervasiveDataInfo.DynamicParameters.Add("usr_pass", homesUser.Password);
            pervasiveDataInfo.DynamicParameters.Add("usr_pft_ctr", homesUser.ProfitCenter);
            pervasiveDataInfo.DynamicParameters.Add("usr_spins", homesUser.Spins);
            pervasiveDataInfo.DynamicParameters.Add("usr_sec_lvl", homesUser.SecurityLevel);
            if (string.IsNullOrEmpty(homesUser.SalesPersonId))
                pervasiveDataInfo.DynamicParameters.Add("usr_slm", string.Empty);
            else
                pervasiveDataInfo.DynamicParameters.Add("usr_slm", homesUser.SalesPersonId);
            pervasiveDataInfo.DynamicParameters.Add("usr_scrn_res_flg", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_welcome_scrn", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_use_drw_flg", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_drw", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_beg_balance", 0.0);
            pervasiveDataInfo.DynamicParameters.Add("usr_coa_budgn_comps", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_st_prt_aging", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_st_prt_zero_bal", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_st_prt_cr_bal", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_st_prt_avl_cr", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_st_prt_ins_info", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_st_run_day", 28);
            pervasiveDataInfo.DynamicParameters.Add("usr_st_cycs_to_proc", string.Empty);
            pervasiveDataInfo.DynamicParameters.Add("usr_st_min_bil", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_st_prt_comp_nam", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_st_even_dol_pmt", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_age_dtl_summ", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_age_use_his_fil", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_age_day", 28);
            pervasiveDataInfo.DynamicParameters.Add("usr_age_anal_st_day", 1);
            pervasiveDataInfo.DynamicParameters.Add("usr_age_itmspst_dat", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_age_prtpr_dtl", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_agecutof_baldue", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_agecutof_prd", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_age_only_cr_bal", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_age_prtzero_bal", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_ageignr_rsv_pmt", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_age_prt_cr_lim", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_age_prtanl_only", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_age_prt_rec_age", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_age_prt_addr", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_age_prt_remarks", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_age_seq", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_pt_first_prc", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_pt_1st_prc_lit", string.Empty);
            pervasiveDataInfo.DynamicParameters.Add("usr_pt_encr_1st_prc", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_pt_2nd_prc", 2);
            pervasiveDataInfo.DynamicParameters.Add("usr_pt_2nd_prc_lit", string.Empty);
            pervasiveDataInfo.DynamicParameters.Add("usr_pt_encr_2nd_prc", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_pt_3rd_prc", 3);
            pervasiveDataInfo.DynamicParameters.Add("usr_pt_3rd_prc_lit", string.Empty);
            pervasiveDataInfo.DynamicParameters.Add("usr_pt_encr_3rd_prc", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_pt_cost", 1);
            pervasiveDataInfo.DynamicParameters.Add("usr_pt_cost_lit", string.Empty);
            pervasiveDataInfo.DynamicParameters.Add("usr_pt_encr_cost", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_pt_bldg_to_prt", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_pt_lbls_across", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_pt_qty_to_prt", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_pt_qty_ohor_loc", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_pt_prt_aux_desc", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_pt_prt_comp_nam", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_ss_alpha_or_num", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_ss_cost_or_loc", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_ss_dtl_or_sum", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_ss_skipzerox_bo", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_ss_skipzeron_bo", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_ss_skipzerob_bo", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_ss_prt_prc_1", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_ss_prt_prc_2", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_ss_prt_prc_3", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_ss_prtonlyw_loc", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_ss_prtavg_w_loc", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_ss_pag_break", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_invcprt_compnam", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_invcprt_pkg_rec", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_invcprt_pkg_prc", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_prt_pkg_item", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_prtpkg_item_prc", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_prt_comp_adrs", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_dr_prt_pkg_rec", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_dr_prt_pkg_prc", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_dr_prt_item_prc", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_crprt_rcpt_dflt", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_cr_auto_tear", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_crprt_rcpt_item", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_cr_prt_rcpt_slm", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_cr_sub_tot", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_poprt_form_cost", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_po_prt_prt_cost", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_po_prt_seq", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_po_prt_comp_nam", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_poprt_comp_adrs", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_poprt_slsperson", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_po_prt_cust_inf", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_so_pak_list_flg", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_soauto_upd_cust", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_so_prt_ins_form", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_soprt_cntr_form", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_so_prt_so", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_so_prt_del_rcp", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_soreserve_to_po", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_so_auto_gen_po", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_so_man_assign", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_so_auto_assign", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_so_dp_dflt", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_delrcp", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_sobo_on_shp_lbl", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_so_del_rcp_tax", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_so_del_rcp_ul", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_so_prt_desc_1", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_del_rcp_abbrev", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_pkg_eq_items", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_item_remarks", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_coll_bookmarks", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_coll_view", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_bc_printer", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_bc_prc_flg", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_tb_buttons", string.Empty);
            pervasiveDataInfo.DynamicParameters.Add("usr_lsr_ar_form_flg", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_new_so_form_flg", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_so_lf_sect1", 3);
            pervasiveDataInfo.DynamicParameters.Add("usr_so_lf_sect2", 1);
            pervasiveDataInfo.DynamicParameters.Add("usr_so_lf_sect3", 3);
            pervasiveDataInfo.DynamicParameters.Add("usr_so_lf_sect4", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_po_lf_sect1", 4);
            pervasiveDataInfo.DynamicParameters.Add("usr_po_lf_sect2", 2);
            pervasiveDataInfo.DynamicParameters.Add("usr_po_lf_sect3", 3);
            pervasiveDataInfo.DynamicParameters.Add("usr_ap_lf_sect1", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_ap_lf_sect2", 3);
            pervasiveDataInfo.DynamicParameters.Add("usr_ap_lf_sect3", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_ap_lf_sect4", 2);
            pervasiveDataInfo.DynamicParameters.Add("usr_ar_lf_sect1", 3);
            pervasiveDataInfo.DynamicParameters.Add("usr_ar_lf_sect2", 4);
            pervasiveDataInfo.DynamicParameters.Add("usr_ar_lf_sect3", 3);
            pervasiveDataInfo.DynamicParameters.Add("usr_ar_lf_sect4", 2);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_cr_appr", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_csh", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_csh_drw", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_mrktg", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_billg", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_tax", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_bill2", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_comm", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_del2", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_delivery", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_terms", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_totals", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_so_dtl", 30);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_pft_ctr", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_prt", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_sale_dat", 10);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_slm_1", 20);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_del_dat", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_so_addl_rem", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_typ", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_xtra_2", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_xtra_3", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_xtra_4", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_xtra_5", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_xtra_6", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_xtra_7", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_xtra_8", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_xtra_9", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_seq_xtra_10", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_ascii_xml", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_ap_dist_popup", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_asap_dflt", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_serial_on_form", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_ask_prev_po", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_prt_ap_rem", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_prt_dat_n_disc", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_prt_chk_no", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_lsr_ap_chk", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_v_studio", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_prt_so_curr_dat", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_so_prt_opt", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_so_prt_so_rem", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_bc_printer_typ", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_so_prt_tax_rt", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_so_prt_fabric", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_so_prt_frame", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_so_prt_finish", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_so_prt_trim", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_so_prt_other", 0);
            pervasiveDataInfo.DynamicParameters.Add("usr_filler_1", string.Empty);
            pervasiveDataInfo.DynamicParameters.Add("usr_filler_2", string.Empty);

            StringContent pervasiveDataInfoStringContent = new StringContent(JsonConvert.SerializeObject(pervasiveDataInfo), Encoding.UTF8, contentType);

            response = PervasiveApiClient.PostAsync(_configuration.PervasiveApi + "/api/PervasiveDB", pervasiveDataInfoStringContent).Result;

            if (!response.IsSuccessStatusCode)
            {
                HomesUserException exception = new HomesUserException("homes-users AddAsync not successful. Pervasive API returned " + response.StatusCode.ToString());
                _logger.LogWarning(exception, exception.Message);
                throw exception;
            }


            return homesUser;
        }


        public Task DeleteFromUser(long userId)
        {
            throw new NotImplementedException();
        }

        public bool IsPervasiveTrue()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HomesUser>> GetByIdAsync(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HomesUser>> GetByIdAsync(long Id)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HomesUser>> GetAsync(HomesUser hmsDomainEntity)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HomesUser>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<HomesUser> UpdateAsync(HomesUser hmsTransaction)
        {
            throw new NotImplementedException();
        }

       

        public Task<bool> DeleteAsync(HomesUser hmsTransaction)
        {
            throw new NotImplementedException();
        }
    }
}
