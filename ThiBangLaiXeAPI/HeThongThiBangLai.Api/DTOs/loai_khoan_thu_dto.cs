namespace HeThongThiBangLai.Api.DTOs;

public class loai_khoan_thu_dto
{
    public long id { get; set; }
    public string ma_loai { get; set; }
    public string ten_loai { get; set; }
    public decimal so_tien_mac_dinh { get; set; }
    public string? mo_ta { get; set; }
    public string trang_thai { get; set; }
}
