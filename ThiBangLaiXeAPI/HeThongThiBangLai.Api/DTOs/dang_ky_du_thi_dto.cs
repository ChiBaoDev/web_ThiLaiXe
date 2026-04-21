namespace HeThongThiBangLai.Api.DTOs;

public class dang_ky_du_thi_dto
{
    public long id { get; set; }
    public long hoc_vien_id { get; set; }
    public long ca_thi_id { get; set; }
    public DateTime ngay_dang_ky { get; set; }
    public string trang_thai { get; set; }
    public long? nguoi_duyet_id { get; set; }
    public DateTime? ngay_duyet { get; set; }
}
