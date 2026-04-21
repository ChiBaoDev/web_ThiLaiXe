namespace HeThongThiBangLai.Api.DTOs;

public class chi_tiet_phieu_thu_dto
{
    public long id { get; set; }
    public long phieu_thu_id { get; set; }
    public long loai_khoan_thu_id { get; set; }
    public decimal so_tien { get; set; }
    public string? ghi_chu { get; set; }
}
