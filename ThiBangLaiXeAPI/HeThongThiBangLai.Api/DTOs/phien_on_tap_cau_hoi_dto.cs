namespace HeThongThiBangLai.Api.DTOs;

public class phien_on_tap_cau_hoi_dto
{
    public long id { get; set; }
    public long phien_on_tap_id { get; set; }
    public long cau_hoi_id { get; set; }
    public long? dap_an_chon_id { get; set; }
    public bool? la_dung { get; set; }
    public int thu_tu_cau { get; set; }
}
