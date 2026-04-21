namespace HeThongThiBangLai.Api.DTOs;

public class giay_to_dinh_kem_dto
{
    public long id { get; set; }
    public long ho_so_id { get; set; }
    public string ten_giay_to { get; set; }
    public string duong_dan_file { get; set; }
    public string? loai_file { get; set; }
    public DateTime ngay_tai_len { get; set; }
    public string trang_thai { get; set; }
}
