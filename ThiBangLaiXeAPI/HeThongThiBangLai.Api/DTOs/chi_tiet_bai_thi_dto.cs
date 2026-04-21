namespace HeThongThiBangLai.Api.DTOs;

public class chi_tiet_bai_thi_dto
{
    public long id { get; set; }
    public long bai_thi_id { get; set; }
    public long cau_hoi_id { get; set; }
    public long? dap_an_chon_id { get; set; }
    public bool? la_dung { get; set; }
}
