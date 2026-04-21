namespace HeThongThiBangLai.Api.DTOs;

public class bai_thi_dto
{
    public long id { get; set; }
    public long hoc_vien_id { get; set; }
    public long de_thi_id { get; set; }
    public long ca_thi_id { get; set; }
    public DateTime? thoi_gian_bat_dau { get; set; }
    public DateTime? thoi_gian_nop { get; set; }
    public int tong_so_cau { get; set; }
    public int so_cau_dung { get; set; }
    public decimal diem { get; set; }
    public string? ket_qua { get; set; }
    public string trang_thai { get; set; }
}
