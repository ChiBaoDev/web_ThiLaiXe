namespace HeThongThiBangLai.Api.DTOs;

public class vi_pham_quy_che_dto
{
    public long id { get; set; }
    public long hoc_vien_id { get; set; }
    public long? bai_thi_id { get; set; }
    public long loai_vi_pham_id { get; set; }
    public long? nguoi_ghi_nhan_id { get; set; }
    public DateTime thoi_gian_vi_pham { get; set; }
    public string? mo_ta { get; set; }
    public string? hinh_thuc_xu_ly { get; set; }
}
