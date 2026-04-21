namespace HeThongThiBangLai.Api.DTOs;

public class lop_hoc_dto
{
    public long id { get; set; }
    public long khoa_hoc_id { get; set; }
    public string ma_lop { get; set; }
    public string ten_lop { get; set; }
    public long? giao_vien_id { get; set; }
    public DateOnly? ngay_bat_dau { get; set; }
    public DateOnly? ngay_ket_thuc { get; set; }
    public int si_so_toi_da { get; set; }
    public string trang_thai { get; set; }
}
