namespace HeThongThiBangLai.Api.DTOs;

public class phieu_thu_dto
{
    public long id { get; set; }
    public string ma_phieu_thu { get; set; }
    public long hoc_vien_id { get; set; }
    public DateTime ngay_thu { get; set; }
    public decimal tong_tien { get; set; }
    public string trang_thai { get; set; }
    public long? nguoi_lap_id { get; set; }
    public long? nguoi_xac_nhan_id { get; set; }
}
