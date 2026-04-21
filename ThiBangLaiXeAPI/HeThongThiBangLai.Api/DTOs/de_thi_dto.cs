namespace HeThongThiBangLai.Api.DTOs;

public class de_thi_dto
{
    public long id { get; set; }
    public string ma_de_thi { get; set; }
    public string ten_de_thi { get; set; }
    public long ky_thi_id { get; set; }
    public int tong_so_cau { get; set; }
    public int thoi_gian_lam_bai { get; set; }
    public string trang_thai { get; set; }
    public long? nguoi_tao_id { get; set; }
    public DateTime ngay_tao { get; set; }
}
