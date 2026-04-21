namespace HeThongThiBangLai.Api.DTOs;

public class ca_thi_dto
{
    public long id { get; set; }
    public long ky_thi_id { get; set; }
    public string ma_ca_thi { get; set; }
    public string ten_ca_thi { get; set; }
    public TimeOnly gio_bat_dau { get; set; }
    public TimeOnly gio_ket_thuc { get; set; }
    public string? phong_thi { get; set; }
    public int so_luong_toi_da { get; set; }
}
