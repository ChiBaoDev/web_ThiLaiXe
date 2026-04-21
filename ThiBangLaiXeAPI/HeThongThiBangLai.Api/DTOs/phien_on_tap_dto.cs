namespace HeThongThiBangLai.Api.DTOs;

public class phien_on_tap_dto
{
    public long id { get; set; }
    public long hoc_vien_id { get; set; }
    public DateTime ngay_tao { get; set; }
    public DateTime? thoi_gian_bat_dau { get; set; }
    public DateTime? thoi_gian_nop { get; set; }
    public int tong_so_cau { get; set; }
    public int so_cau_dung { get; set; }
    public decimal diem { get; set; }
    public string trang_thai { get; set; }
}
