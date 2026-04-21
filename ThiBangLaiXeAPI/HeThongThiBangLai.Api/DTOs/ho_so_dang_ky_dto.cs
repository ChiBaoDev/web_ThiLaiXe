namespace HeThongThiBangLai.Api.DTOs;

public class ho_so_dang_ky_dto
{
    public long id { get; set; }
    public long hoc_vien_id { get; set; }
    public string ma_ho_so { get; set; }
    public DateTime? ngay_nop { get; set; }
    public string trang_thai { get; set; }
    public string? ghi_chu { get; set; }
    public long? nguoi_duyet_id { get; set; }
    public DateTime? ngay_duyet { get; set; }
}
