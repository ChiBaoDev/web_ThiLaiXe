namespace HeThongThiBangLai.Api.DTOs;

public class hoc_vien_dto
{
    public long id { get; set; }
    public long nguoi_dung_id { get; set; }
    public string ho_ten { get; set; }
    public DateOnly? ngay_sinh { get; set; }
    public string? gioi_tinh { get; set; }
    public string? cccd { get; set; }
    public string? dia_chi { get; set; }
    public string? anh_chan_dung { get; set; }
    public DateTime created_at { get; set; }
}
