namespace HeThongThiBangLai.Api.DTOs;

public class lop_hoc_hoc_vien_dto
{
    public long id { get; set; }
    public long lop_hoc_id { get; set; }
    public long hoc_vien_id { get; set; }
    public DateOnly? ngay_vao_lop { get; set; }
    public string trang_thai { get; set; }
}
