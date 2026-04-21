/*
    File: 02_seed_exam_a1a_from_pdf.sql
    Muc tieu:
    - Seed du lieu mau logic theo bo 250 cau hoi A1/A (co cau diem liet)
    - Tao topic, cau hoi, dap an, ky thi/ca thi/de thi mau 25 cau
    - Seed exam structure rule (dang log he thong) de API /exam-structure-rules doc duoc

    Ghi chu quan trong:
    - Script idempotent (co the chay lap lai)
    - De phu hop voi ExamSessionService hien tai (ca_thi_id = ky_thi_id), script tao ky_thi va ca_thi cung ID
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    ------------------------------------------------------------
    -- 0) CHUAN BI TOPIC
    ------------------------------------------------------------
    IF NOT EXISTS (SELECT 1 FROM chu_de_cau_hoi WHERE ma_chu_de = 'CD_QTGT')
        INSERT INTO chu_de_cau_hoi(ma_chu_de, ten_chu_de, mo_ta)
        VALUES ('CD_QTGT', N'Quy tắc giao thông đường bộ', N'Nhóm quy tắc chung và hành vi khi tham gia giao thông');

    IF NOT EXISTS (SELECT 1 FROM chu_de_cau_hoi WHERE ma_chu_de = 'CD_LIET')
        INSERT INTO chu_de_cau_hoi(ma_chu_de, ten_chu_de, mo_ta)
        VALUES ('CD_LIET', N'Tình huống mất an toàn nghiêm trọng (điểm liệt)', N'Câu hỏi trọng yếu, trả lời sai là không đạt');

    IF NOT EXISTS (SELECT 1 FROM chu_de_cau_hoi WHERE ma_chu_de = 'CD_VH')
        INSERT INTO chu_de_cau_hoi(ma_chu_de, ten_chu_de, mo_ta)
        VALUES ('CD_VH', N'Văn hóa và đạo đức lái xe', N'Ứng xử văn minh, trách nhiệm khi tham gia giao thông');

    IF NOT EXISTS (SELECT 1 FROM chu_de_cau_hoi WHERE ma_chu_de = 'CD_KT')
        INSERT INTO chu_de_cau_hoi(ma_chu_de, ten_chu_de, mo_ta)
        VALUES ('CD_KT', N'Kỹ thuật lái xe và cấu tạo', N'Kiến thức kỹ thuật cơ bản đối với xe mô tô');

    IF NOT EXISTS (SELECT 1 FROM chu_de_cau_hoi WHERE ma_chu_de = 'CD_BH')
        INSERT INTO chu_de_cau_hoi(ma_chu_de, ten_chu_de, mo_ta)
        VALUES ('CD_BH', N'Hệ thống báo hiệu đường bộ', N'Biển báo, vạch kẻ đường, tín hiệu đèn, hiệu lệnh');

    IF NOT EXISTS (SELECT 1 FROM chu_de_cau_hoi WHERE ma_chu_de = 'CD_SH')
        INSERT INTO chu_de_cau_hoi(ma_chu_de, ten_chu_de, mo_ta)
        VALUES ('CD_SH', N'Sa hình và tình huống giao thông', N'Ưu tiên, xử lý tình huống thực tế tại nút giao');

    ------------------------------------------------------------
    -- 1) SEED CAU HOI MAU (PHONG THEO PDF 250 CAU)
    ------------------------------------------------------------
    IF OBJECT_ID('tempdb..#QuestionsSeed') IS NOT NULL DROP TABLE #QuestionsSeed;
    CREATE TABLE #QuestionsSeed
    (
        q_code VARCHAR(30) NOT NULL,
        topic_code VARCHAR(30) NOT NULL,
        content NVARCHAR(MAX) NOT NULL,
        level VARCHAR(30) NULL,
        is_critical BIT NOT NULL,
        status VARCHAR(30) NOT NULL
    );

    INSERT INTO #QuestionsSeed(q_code, topic_code, content, level, is_critical, status)
    VALUES
    -- Quy tac chung (10)
    ('Q001','CD_QTGT',N'[PDF-A1A-001] Người điều khiển xe mô tô phải mang theo giấy tờ nào khi tham gia giao thông?', 'de', 0, 'approved'),
    ('Q002','CD_QTGT',N'[PDF-A1A-002] Khi chuyển hướng, người lái xe phải thực hiện tín hiệu báo rẽ trong trường hợp nào?', 'de', 0, 'approved'),
    ('Q003','CD_QTGT',N'[PDF-A1A-003] Khi đi gần xe buýt đang dừng đón trả khách, người lái xe cần làm gì?', 'tb', 0, 'approved'),
    ('Q004','CD_QTGT',N'[PDF-A1A-004] Người lái xe mô tô được phép chở tối đa bao nhiêu người trong trường hợp thông thường?', 'de', 0, 'approved'),
    ('Q005','CD_QTGT',N'[PDF-A1A-005] Khi gặp người đi bộ đang qua đường tại nơi có vạch dành cho người đi bộ, người lái xe phải làm gì?', 'de', 0, 'approved'),
    ('Q006','CD_QTGT',N'[PDF-A1A-006] Trong khu đông dân cư, người lái xe cần chú ý điều gì để bảo đảm an toàn?', 'tb', 0, 'approved'),
    ('Q007','CD_QTGT',N'[PDF-A1A-007] Khi đi trong điều kiện trời mưa, tầm nhìn hạn chế, cách xử lý phù hợp là gì?', 'tb', 0, 'approved'),
    ('Q008','CD_QTGT',N'[PDF-A1A-008] Khi có tín hiệu xin vượt của xe phía sau, người lái xe phía trước cần ứng xử thế nào?', 'tb', 0, 'approved'),
    ('Q009','CD_QTGT',N'[PDF-A1A-009] Người điều khiển xe mô tô có được sử dụng điện thoại bằng tay khi đang lái xe không?', 'de', 0, 'approved'),
    ('Q010','CD_QTGT',N'[PDF-A1A-010] Trường hợp nào người lái xe phải giảm tốc độ và chú ý quan sát đặc biệt?', 'tb', 0, 'approved'),

    -- Cau diem liet (4)
    ('Q011','CD_LIET',N'[PDF-A1A-011][CÂU LIỆT] Hành vi điều khiển xe mô tô đi ngược chiều trên đường cao tốc có được phép không?', 'kho', 1, 'approved'),
    ('Q012','CD_LIET',N'[PDF-A1A-012][CÂU LIỆT] Người lái xe sử dụng rượu bia vượt nồng độ quy định khi điều khiển xe mô tô sẽ bị xử lý thế nào?', 'kho', 1, 'approved'),
    ('Q013','CD_LIET',N'[PDF-A1A-013][CÂU LIỆT] Đua xe trái phép trên đường bộ có phải hành vi bị nghiêm cấm không?', 'kho', 1, 'approved'),
    ('Q014','CD_LIET',N'[PDF-A1A-014][CÂU LIỆT] Không chấp hành hiệu lệnh dừng xe của người thi hành công vụ là hành vi như thế nào?', 'kho', 1, 'approved'),

    -- Van hoa dao duc (2)
    ('Q015','CD_VH',N'[PDF-A1A-015] Người lái xe văn minh cần ứng xử thế nào khi xảy ra va chạm giao thông?', 'de', 0, 'approved'),
    ('Q016','CD_VH',N'[PDF-A1A-016] Trách nhiệm của người gây tai nạn giao thông là gì?', 'tb', 0, 'approved'),

    -- Ky thuat cau tao (3)
    ('Q017','CD_KT',N'[PDF-A1A-017] Trước khi khởi hành, người lái xe cần kiểm tra nội dung kỹ thuật nào?', 'de', 0, 'approved'),
    ('Q018','CD_KT',N'[PDF-A1A-018] Lốp xe bị mòn quá giới hạn ảnh hưởng như thế nào đến an toàn?', 'tb', 0, 'approved'),
    ('Q019','CD_KT',N'[PDF-A1A-019] Phanh trước và phanh sau cần sử dụng ra sao để đạt hiệu quả an toàn?', 'tb', 0, 'approved'),

    -- Bao hieu duong bo (10)
    ('Q020','CD_BH',N'[PDF-A1A-020] Biển tròn viền đỏ nền trắng thường thể hiện nhóm biển gì?', 'de', 0, 'approved'),
    ('Q021','CD_BH',N'[PDF-A1A-021] Biển nền xanh hình chữ nhật thường thuộc nhóm biển nào?', 'de', 0, 'approved'),
    ('Q022','CD_BH',N'[PDF-A1A-022] Khi đèn tín hiệu giao thông chuyển vàng, người lái xe phải xử lý như thế nào?', 'tb', 0, 'approved'),
    ('Q023','CD_BH',N'[PDF-A1A-023] Vạch liền màu trắng phân chia làn xe có ý nghĩa gì?', 'tb', 0, 'approved'),
    ('Q024','CD_BH',N'[PDF-A1A-024] Gặp biển “Cấm đi ngược chiều”, người lái xe được phép làm gì?', 'de', 0, 'approved'),
    ('Q025','CD_BH',N'[PDF-A1A-025] Biển báo nguy hiểm có dạng hình học phổ biến nào?', 'de', 0, 'approved'),
    ('Q026','CD_BH',N'[PDF-A1A-026] Ý nghĩa của biển “Đường ưu tiên” là gì?', 'tb', 0, 'approved'),
    ('Q027','CD_BH',N'[PDF-A1A-027] Khi gặp hiệu lệnh của người điều khiển giao thông khác với đèn tín hiệu, phải chấp hành theo gì?', 'tb', 0, 'approved'),
    ('Q028','CD_BH',N'[PDF-A1A-028] Biển “Dừng lại” yêu cầu người lái xe thực hiện hành động nào?', 'de', 0, 'approved'),
    ('Q029','CD_BH',N'[PDF-A1A-029] Khi thấy biển chỉ dẫn hướng đi trên đường một chiều, người lái xe cần làm gì?', 'de', 0, 'approved'),

    -- Sa hinh tinh huong (8)
    ('Q030','CD_SH',N'[PDF-A1A-030] Tại ngã tư không có đèn tín hiệu, xe đến từ bên phải có được ưu tiên không?', 'tb', 0, 'approved'),
    ('Q031','CD_SH',N'[PDF-A1A-031] Khi rẽ trái tại giao lộ, người lái xe phải nhường đường cho đối tượng nào?', 'tb', 0, 'approved'),
    ('Q032','CD_SH',N'[PDF-A1A-032] Khi vào vòng xuyến, người lái xe phải nhường đường cho xe nào?', 'tb', 0, 'approved'),
    ('Q033','CD_SH',N'[PDF-A1A-033] Gặp xe ưu tiên đang phát tín hiệu làm nhiệm vụ, người lái xe phải xử lý ra sao?', 'de', 0, 'approved'),
    ('Q034','CD_SH',N'[PDF-A1A-034] Khi qua đường sắt không có rào chắn, người lái xe cần thực hiện nguyên tắc nào?', 'tb', 0, 'approved'),
    ('Q035','CD_SH',N'[PDF-A1A-035] Ở nơi đường hẹp chỉ đủ một xe, gặp xe đi ngược chiều thì xử lý thế nào?', 'kho', 0, 'approved'),
    ('Q036','CD_SH',N'[PDF-A1A-036] Khi lùi xe trong khu vực đông người, nguyên tắc an toàn là gì?', 'tb', 0, 'approved'),
    ('Q037','CD_SH',N'[PDF-A1A-037] Khi chuyển làn liên tục trước nút giao đông đúc có phù hợp quy tắc an toàn không?', 'tb', 0, 'approved');

    DECLARE @TopicMap TABLE(topic_code VARCHAR(30) PRIMARY KEY, topic_id BIGINT NOT NULL);
    INSERT INTO @TopicMap(topic_code, topic_id)
    SELECT ma_chu_de, id
    FROM chu_de_cau_hoi
    WHERE ma_chu_de IN ('CD_QTGT','CD_LIET','CD_VH','CD_KT','CD_BH','CD_SH');

    INSERT INTO cau_hoi(chu_de_id, noi_dung, loai_cau_hoi, muc_do, la_cau_diem_liet, trang_thai)
    SELECT tm.topic_id, q.content, 'trac_nghiem', q.level, q.is_critical, q.status
    FROM #QuestionsSeed q
    INNER JOIN @TopicMap tm ON tm.topic_code = q.topic_code
    WHERE NOT EXISTS (
        SELECT 1
        FROM cau_hoi c
        WHERE c.noi_dung = q.content
    );

    ------------------------------------------------------------
    -- 2) SEED DAP AN (4 LUA CHON / CAU, 1 DAP AN DUNG)
    ------------------------------------------------------------
    IF OBJECT_ID('tempdb..#AnswersSeed') IS NOT NULL DROP TABLE #AnswersSeed;
    CREATE TABLE #AnswersSeed
    (
        q_code VARCHAR(30) NOT NULL,
        answer_order INT NOT NULL,
        answer_content NVARCHAR(1000) NOT NULL,
        is_correct BIT NOT NULL
    );

    INSERT INTO #AnswersSeed(q_code, answer_order, answer_content, is_correct)
    VALUES
    ('Q001',1,N'Giấy đăng ký xe và bảo hiểm bắt buộc',0),('Q001',2,N'Giấy phép lái xe phù hợp, đăng ký xe, bảo hiểm bắt buộc',1),('Q001',3,N'Chỉ cần căn cước công dân',0),('Q001',4,N'Không cần giấy tờ nếu chạy gần nhà',0),
    ('Q002',1,N'Chỉ bật xi nhan sau khi đã chuyển hướng xong',0),('Q002',2,N'Báo rẽ trước khi chuyển hướng và quan sát an toàn',1),('Q002',3,N'Không cần báo rẽ khi đường vắng',0),('Q002',4,N'Bấm còi thay cho xi nhan',0),
    ('Q003',1,N'Vượt nhanh để tránh ùn tắc',0),('Q003',2,N'Giảm tốc độ, quan sát người lên xuống xe buýt',1),('Q003',3,N'Đi sát hông xe buýt',0),('Q003',4,N'Bật đèn pha liên tục',0),
    ('Q004',1,N'Chở tối đa 01 người trong trường hợp thông thường',1),('Q004',2,N'Chở tối đa 02 người bất kỳ lúc nào',0),('Q004',3,N'Chở bao nhiêu tùy xe',0),('Q004',4,N'Chỉ cần đội mũ bảo hiểm là được chở 2 người',0),
    ('Q005',1,N'Tăng tốc để đi qua trước',0),('Q005',2,N'Bấm còi liên tục để người đi bộ tránh',0),('Q005',3,N'Giảm tốc, nhường đường cho người đi bộ',1),('Q005',4,N'Đi sát vạch để gây chú ý',0),
    ('Q006',1,N'Luôn giữ tốc độ tối đa cho phép',0),('Q006',2,N'Chủ động giảm tốc, giữ khoảng cách an toàn',1),('Q006',3,N'Đi sát xe trước để tránh chen ngang',0),('Q006',4,N'Chỉ quan sát xe máy cùng chiều',0),
    ('Q007',1,N'Giảm tốc độ, bật đèn chiếu gần và tăng quan sát',1),('Q007',2,N'Đi nhanh qua đoạn mưa',0),('Q007',3,N'Bật đèn pha để nhìn rõ hơn mọi lúc',0),('Q007',4,N'Phanh gấp khi thấy vũng nước',0),
    ('Q008',1,N'Tăng tốc để không cho vượt',0),('Q008',2,N'Giữ ổn định, đi về bên phải khi đủ an toàn',1),('Q008',3,N'Đánh lái sang trái để kiểm soát',0),('Q008',4,N'Bóp còi phản đối',0),
    ('Q009',1,N'Được phép nếu đi chậm',0),('Q009',2,N'Được phép nếu nghe loa ngoài',0),('Q009',3,N'Không được sử dụng điện thoại bằng tay khi lái xe',1),('Q009',4,N'Chỉ cấm khi đi ban đêm',0),
    ('Q010',1,N'Khi đường trống và tầm nhìn tốt',0),('Q010',2,N'Khi gần trường học, bệnh viện, nơi đông dân cư',1),('Q010',3,N'Khi chạy trên đường ưu tiên',0),('Q010',4,N'Khi vừa vượt xe khác xong',0),

    ('Q011',1,N'Được phép nếu vắng xe',0),('Q011',2,N'Chỉ được phép vào ban đêm',0),('Q011',3,N'Tuyệt đối không được phép',1),('Q011',4,N'Được phép khi có xe dẫn đường',0),
    ('Q012',1,N'Không bị xử lý nếu chạy chậm',0),('Q012',2,N'Bị xử lý nghiêm theo quy định hiện hành',1),('Q012',3,N'Chỉ nhắc nhở lần đầu',0),('Q012',4,N'Chỉ xử phạt khi gây tai nạn',0),
    ('Q013',1,N'Không bị cấm nếu đội mũ bảo hiểm',0),('Q013',2,N'Là hành vi bị nghiêm cấm',1),('Q013',3,N'Được phép tại đường vắng',0),('Q013',4,N'Được phép nếu xe phân khối lớn',0),
    ('Q014',1,N'Được phép nếu đang bận việc',0),('Q014',2,N'Không nghiêm trọng nếu chưa gây tai nạn',0),('Q014',3,N'Là hành vi vi phạm pháp luật giao thông',1),('Q014',4,N'Chỉ bị nhắc nhở',0),

    ('Q015',1,N'Rời khỏi hiện trường ngay',0),('Q015',2,N'Giữ bình tĩnh, hỗ trợ người bị nạn và báo cơ quan chức năng',1),('Q015',3,N'Tranh cãi để xác định đúng sai trước',0),('Q015',4,N'Đăng mạng xã hội trước khi xử lý',0),
    ('Q016',1,N'Bỏ đi nếu thiệt hại nhỏ',0),('Q016',2,N'Cứu giúp người bị nạn, giữ hiện trường, trình báo',1),('Q016',3,N'Chỉ cần gọi bảo hiểm',0),('Q016',4,N'Chờ người khác xử lý',0),

    ('Q017',1,N'Chỉ cần kiểm tra xăng',0),('Q017',2,N'Kiểm tra phanh, lốp, đèn, còi trước khi đi',1),('Q017',3,N'Không cần kiểm tra nếu xe mới',0),('Q017',4,N'Kiểm tra sau khi đã chạy',0),
    ('Q018',1,N'Không ảnh hưởng nếu trời khô ráo',0),('Q018',2,N'Làm giảm độ bám đường, tăng nguy cơ trượt ngã',1),('Q018',3,N'Chỉ ảnh hưởng khi chở nặng',0),('Q018',4,N'Giúp tiết kiệm nhiên liệu',0),
    ('Q019',1,N'Chỉ dùng phanh trước ở mọi tình huống',0),('Q019',2,N'Phối hợp phanh trước và sau hợp lý theo tình huống',1),('Q019',3,N'Chỉ dùng phanh sau để an toàn tuyệt đối',0),('Q019',4,N'Không dùng phanh khi xuống dốc',0),

    ('Q020',1,N'Biển chỉ dẫn',0),('Q020',2,N'Biển cấm',1),('Q020',3,N'Biển phụ',0),('Q020',4,N'Biển hiệu lệnh',0),
    ('Q021',1,N'Biển cảnh báo nguy hiểm',0),('Q021',2,N'Biển chỉ dẫn',1),('Q021',3,N'Biển cấm tạm thời',0),('Q021',4,N'Biển hết hạn chế',0),
    ('Q022',1,N'Tăng tốc vượt nhanh qua nút giao',0),('Q022',2,N'Giảm tốc và dừng trước vạch nếu không thể đi qua an toàn',1),('Q022',3,N'Đi tiếp không cần quan sát',0),('Q022',4,N'Chỉ dừng khi có CSGT',0),
    ('Q023',1,N'Được phép đè vạch để vượt',0),('Q023',2,N'Không được lấn, đè vạch liền phân chia làn',1),('Q023',3,N'Chỉ cấm xe tải',0),('Q023',4,N'Chỉ áp dụng ban ngày',0),
    ('Q024',1,N'Được quay đầu ngay sau biển',0),('Q024',2,N'Không được đi vào theo chiều bị cấm',1),('Q024',3,N'Được đi nếu bật cảnh báo',0),('Q024',4,N'Được đi với tốc độ thấp',0),
    ('Q025',1,N'Hình tròn nền xanh',0),('Q025',2,N'Hình tam giác viền đỏ nền vàng/trắng',1),('Q025',3,N'Hình vuông nền trắng',0),('Q025',4,N'Hình chữ nhật viền đỏ',0),
    ('Q026',1,N'Được quyền đi trước tại nơi giao nhau theo quy định',1),('Q026',2,N'Bắt buộc phải dừng mọi lúc',0),('Q026',3,N'Cấm mọi phương tiện khác',0),('Q026',4,N'Đường chỉ dành cho xe máy',0),
    ('Q027',1,N'Luôn theo đèn tín hiệu trước',0),('Q027',2,N'Chấp hành theo hiệu lệnh người điều khiển giao thông',1),('Q027',3,N'Theo biển báo gần nhất',0),('Q027',4,N'Theo xe đi trước',0),
    ('Q028',1,N'Giảm tốc rồi đi tiếp nếu không có xe',0),('Q028',2,N'Phải dừng hẳn trước vạch dừng',1),('Q028',3,N'Chỉ dừng khi có người đi bộ',0),('Q028',4,N'Chỉ dừng ban ngày',0),
    ('Q029',1,N'Đi theo hướng chỉ dẫn và tuân thủ phần đường',1),('Q029',2,N'Có thể đi ngược chiều nếu gần đích',0),('Q029',3,N'Được tùy chọn làn theo ý muốn',0),('Q029',4,N'Không cần quan tâm vì chỉ mang tính tham khảo',0),

    ('Q030',1,N'Không, luôn ưu tiên xe đi trước bất kể hướng',0),('Q030',2,N'Có, theo nguyên tắc nhường đường tại nơi giao nhau',1),('Q030',3,N'Chỉ ưu tiên xe rẽ trái',0),('Q030',4,N'Xe máy không áp dụng quy tắc này',0),
    ('Q031',1,N'Nhường đường cho xe đi ngược chiều đi thẳng hoặc rẽ phải',1),('Q031',2,N'Không cần nhường vì đã bật xi nhan',0),('Q031',3,N'Chỉ nhường ô tô',0),('Q031',4,N'Luôn đi trước xe đạp',0),
    ('Q032',1,N'Nhường xe chuẩn bị vào vòng xuyến',0),('Q032',2,N'Nhường xe đang lưu thông trong vòng xuyến',1),('Q032',3,N'Xe lớn phải nhường xe nhỏ',0),('Q032',4,N'Không có quy tắc ưu tiên',0),
    ('Q033',1,N'Tăng tốc vượt trước để tránh cản đường',0),('Q033',2,N'Nhanh chóng giảm tốc, đi sát lề phải và nhường đường',1),('Q033',3,N'Bấm còi xin đi trước',0),('Q033',4,N'Giữ nguyên tốc độ vì đã đúng làn',0),
    ('Q034',1,N'Qua nhanh để tránh tàu',0),('Q034',2,N'Giảm tốc, quan sát kỹ, chỉ đi khi bảo đảm an toàn',1),('Q034',3,N'Bấm còi liên tục rồi đi',0),('Q034',4,N'Đi theo xe phía trước mà không cần quan sát',0),
    ('Q035',1,N'Xe xuống dốc phải nhường xe lên dốc ở đoạn hẹp',1),('Q035',2,N'Xe nào còi to hơn được đi trước',0),('Q035',3,N'Xe máy luôn được ưu tiên',0),('Q035',4,N'Không cần nhường nếu đang đúng làn',0),
    ('Q036',1,N'Lùi xe nhanh để giảm thời gian cản trở',0),('Q036',2,N'Quan sát kỹ, lùi chậm, bảo đảm không gây nguy hiểm',1),('Q036',3,N'Chỉ cần nhìn gương trái',0),('Q036',4,N'Không cần tín hiệu khi lùi',0),
    ('Q037',1,N'Phù hợp vì giúp vượt xe nhanh',0),('Q037',2,N'Không phù hợp, dễ gây mất an toàn và xung đột giao thông',1),('Q037',3,N'Chỉ cấm với ô tô',0),('Q037',4,N'Được phép nếu bật còi',0);

    DECLARE @QuestionMap TABLE(q_code VARCHAR(30) PRIMARY KEY, question_id BIGINT NOT NULL);
    INSERT INTO @QuestionMap(q_code, question_id)
    SELECT qs.q_code, c.id
    FROM #QuestionsSeed qs
    INNER JOIN cau_hoi c ON c.noi_dung = qs.content;

    MERGE dap_an AS target
    USING (
        SELECT qm.question_id, a.answer_order, a.answer_content, a.is_correct
        FROM #AnswersSeed a
        INNER JOIN @QuestionMap qm ON qm.q_code = a.q_code
    ) AS src
    ON target.cau_hoi_id = src.question_id
       AND target.thu_tu = src.answer_order
    WHEN MATCHED THEN
        UPDATE SET
            target.noi_dung = src.answer_content,
            target.la_dap_an_dung = src.is_correct
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (cau_hoi_id, noi_dung, la_dap_an_dung, thu_tu)
        VALUES (src.question_id, src.answer_content, src.is_correct, src.answer_order);

    ------------------------------------------------------------
    -- 3) TAO KY THI + CA THI COMPATIBLE ID
    ------------------------------------------------------------
    DECLARE @CompatExamId BIGINT = 260401; -- ID co dinh de bao dam ky_thi.id = ca_thi.id

    IF EXISTS (SELECT 1 FROM ky_thi WHERE id = @CompatExamId)
    BEGIN
        UPDATE ky_thi
        SET ma_ky_thi = 'KYTHI_A1A_PDF_2026',
            ten_ky_thi = N'Kỳ thi A1/A mô phỏng theo bộ 250 câu',
            ngay_thi = CAST(GETDATE() AS DATE),
            mo_ta = N'Dữ liệu test theo cấu trúc đề 25 câu có câu điểm liệt',
            trang_thai = 'dang_dien_ra'
        WHERE id = @CompatExamId;
    END
    ELSE
    BEGIN
        SET IDENTITY_INSERT ky_thi ON;
        INSERT INTO ky_thi(id, ma_ky_thi, ten_ky_thi, ngay_thi, mo_ta, trang_thai)
        VALUES (@CompatExamId, 'KYTHI_A1A_PDF_2026', N'Kỳ thi A1/A mô phỏng theo bộ 250 câu', CAST(GETDATE() AS DATE), N'Dữ liệu test theo cấu trúc đề 25 câu có câu điểm liệt', 'dang_dien_ra');
        SET IDENTITY_INSERT ky_thi OFF;
    END;

    IF EXISTS (SELECT 1 FROM ca_thi WHERE id = @CompatExamId AND ky_thi_id <> @CompatExamId)
    BEGIN
        THROW 51001, 'ca_thi.id = 260401 da ton tai nhung khong thuoc ky_thi 260401. Hay doi @CompatExamId trong script.', 1;
    END;

    IF EXISTS (SELECT 1 FROM ca_thi WHERE id = @CompatExamId)
    BEGIN
        UPDATE ca_thi
        SET ky_thi_id = @CompatExamId,
            ma_ca_thi = 'CA_A1A_PDF_01',
            ten_ca_thi = N'Ca thi mô phỏng A1/A',
            gio_bat_dau = '08:00:00',
            gio_ket_thuc = '08:30:00',
            phong_thi = N'Phòng mô phỏng API',
            so_luong_toi_da = 200
        WHERE id = @CompatExamId;
    END
    ELSE
    BEGIN
        SET IDENTITY_INSERT ca_thi ON;
        INSERT INTO ca_thi(id, ky_thi_id, ma_ca_thi, ten_ca_thi, gio_bat_dau, gio_ket_thuc, phong_thi, so_luong_toi_da)
        VALUES (@CompatExamId, @CompatExamId, 'CA_A1A_PDF_01', N'Ca thi mô phỏng A1/A', '08:00:00', '08:30:00', N'Phòng mô phỏng API', 200);
        SET IDENTITY_INSERT ca_thi OFF;
    END;

    ------------------------------------------------------------
    -- 4) TAO DE THI MAU 25 CAU / 19 PHUT / PUBLISHED
    ------------------------------------------------------------
    DECLARE @SampleExamCode VARCHAR(30) = 'DE_A1A_PDF_25_01';
    DECLARE @SampleExamId BIGINT;

    IF EXISTS (SELECT 1 FROM de_thi WHERE ma_de_thi = @SampleExamCode)
    BEGIN
        SELECT @SampleExamId = id FROM de_thi WHERE ma_de_thi = @SampleExamCode;

        UPDATE de_thi
        SET ten_de_thi = N'Đề mô phỏng A1/A - 25 câu (PDF)',
            ky_thi_id = @CompatExamId,
            tong_so_cau = 25,
            thoi_gian_lam_bai = 19,
            trang_thai = 'published',
            ngay_tao = GETDATE()
        WHERE id = @SampleExamId;
    END
    ELSE
    BEGIN
        INSERT INTO de_thi(ma_de_thi, ten_de_thi, ky_thi_id, tong_so_cau, thoi_gian_lam_bai, trang_thai, nguoi_tao_id, ngay_tao)
        VALUES (@SampleExamCode, N'Đề mô phỏng A1/A - 25 câu (PDF)', @CompatExamId, 25, 19, 'published', NULL, GETDATE());

        SET @SampleExamId = SCOPE_IDENTITY();
    END;

    -- Cau truc de theo PDF:
    -- 08 quy tac + 01 diem liet + 01 van hoa + 01 ky thuat + 08 bao hieu + 06 sa hinh = 25
    IF OBJECT_ID('tempdb..#ExamQuestionPick') IS NOT NULL DROP TABLE #ExamQuestionPick;
    CREATE TABLE #ExamQuestionPick
    (
        q_code VARCHAR(30) NOT NULL,
        ord INT NOT NULL
    );

    INSERT INTO #ExamQuestionPick(q_code, ord)
    VALUES
    ('Q001',1),('Q002',2),('Q003',3),('Q004',4),('Q005',5),('Q006',6),('Q007',7),('Q008',8), -- 8 quy tac
    ('Q011',9), -- 1 cau diem liet
    ('Q015',10), -- 1 van hoa
    ('Q017',11), -- 1 ky thuat
    ('Q020',12),('Q021',13),('Q022',14),('Q023',15),('Q024',16),('Q025',17),('Q026',18),('Q027',19), -- 8 bao hieu
    ('Q030',20),('Q031',21),('Q032',22),('Q033',23),('Q034',24),('Q035',25); -- 6 sa hinh

    DELETE FROM de_thi_cau_hoi WHERE de_thi_id = @SampleExamId;

    INSERT INTO de_thi_cau_hoi(de_thi_id, cau_hoi_id, thu_tu_cau)
    SELECT @SampleExamId, qm.question_id, p.ord
    FROM #ExamQuestionPick p
    INNER JOIN @QuestionMap qm ON qm.q_code = p.q_code
    ORDER BY p.ord;

    ------------------------------------------------------------
    -- 5) SEED EXAM STRUCTURE RULE (LOG-BASED)
    ------------------------------------------------------------
    DECLARE @RuleId BIGINT = 9001;

    DECLARE @Topic_QTGT BIGINT = (SELECT id FROM chu_de_cau_hoi WHERE ma_chu_de = 'CD_QTGT');
    DECLARE @Topic_LIET BIGINT = (SELECT id FROM chu_de_cau_hoi WHERE ma_chu_de = 'CD_LIET');
    DECLARE @Topic_VH BIGINT = (SELECT id FROM chu_de_cau_hoi WHERE ma_chu_de = 'CD_VH');
    DECLARE @Topic_KT BIGINT = (SELECT id FROM chu_de_cau_hoi WHERE ma_chu_de = 'CD_KT');
    DECLARE @Topic_BH BIGINT = (SELECT id FROM chu_de_cau_hoi WHERE ma_chu_de = 'CD_BH');
    DECLARE @Topic_SH BIGINT = (SELECT id FROM chu_de_cau_hoi WHERE ma_chu_de = 'CD_SH');

    DECLARE @RulePayload NVARCHAR(MAX) =
        N'{'
        + N'"id":' + CAST(@RuleId AS NVARCHAR(20))
        + N',"name":"A1/A theo PDF 250 câu - 25 câu/19 phút"'
        + N',"totalQuestions":25'
        + N',"durationMinutes":19'
        + N',"passingCorrectAnswers":21'
        + N',"requiredCriticalQuestions":1'
        + N',"autoSubmitEnabled":true'
        + N',"criticalFailEnabled":true'
        + N',"isActive":true'
        + N',"updatedAt":"' + CONVERT(NVARCHAR(33), SYSUTCDATETIME(), 126) + N'"'
        + N',"topicAllocations":['
            + N'{"topicId":' + CAST(@Topic_QTGT AS NVARCHAR(20)) + N',"questionCount":8},'
            + N'{"topicId":' + CAST(@Topic_LIET AS NVARCHAR(20)) + N',"questionCount":1},'
            + N'{"topicId":' + CAST(@Topic_VH AS NVARCHAR(20)) + N',"questionCount":1},'
            + N'{"topicId":' + CAST(@Topic_KT AS NVARCHAR(20)) + N',"questionCount":1},'
            + N'{"topicId":' + CAST(@Topic_BH AS NVARCHAR(20)) + N',"questionCount":8},'
            + N'{"topicId":' + CAST(@Topic_SH AS NVARCHAR(20)) + N',"questionCount":6}'
        + N']'
        + N',"difficultyAllocations":['
            + N'{"difficulty":"de","questionCount":10},'
            + N'{"difficulty":"tb","questionCount":11},'
            + N'{"difficulty":"kho","questionCount":4}'
        + N']'
        + N'}';

    INSERT INTO nhat_ky_he_thong(nguoi_dung_id, hanh_dong, bang_tac_dong, khoa_chinh_du_lieu, noi_dung, ip_address, created_at)
    SELECT NULL, 'exam_rule_created', 'exam_structure_rule', @RuleId,
           N'Seeded from PDF 250 câu A1/A | payload=' + @RulePayload,
           '127.0.0.1', SYSUTCDATETIME()
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM nhat_ky_he_thong
        WHERE hanh_dong = 'exam_rule_created'
          AND bang_tac_dong = 'exam_structure_rule'
          AND khoa_chinh_du_lieu = @RuleId
    );

    ------------------------------------------------------------
    -- 6) VERIFY NHANH
    ------------------------------------------------------------
    PRINT '=== SEED SUMMARY ===';
    SELECT ma_chu_de, ten_chu_de FROM chu_de_cau_hoi WHERE ma_chu_de LIKE 'CD_%' ORDER BY ma_chu_de;

    SELECT
        cd.ma_chu_de,
        COUNT(*) AS so_cau,
        SUM(CASE WHEN ch.la_cau_diem_liet = 1 THEN 1 ELSE 0 END) AS so_cau_liet
    FROM cau_hoi ch
    INNER JOIN chu_de_cau_hoi cd ON cd.id = ch.chu_de_id
    WHERE ch.noi_dung LIKE N'[PDF-A1A-%]%'
    GROUP BY cd.ma_chu_de
    ORDER BY cd.ma_chu_de;

    SELECT
        d.id,
        d.ma_de_thi,
        d.ten_de_thi,
        d.tong_so_cau,
        d.thoi_gian_lam_bai,
        d.trang_thai,
        COUNT(dtch.id) AS so_cau_da_gan
    FROM de_thi d
    LEFT JOIN de_thi_cau_hoi dtch ON dtch.de_thi_id = d.id
    WHERE d.ma_de_thi = @SampleExamCode
    GROUP BY d.id, d.ma_de_thi, d.ten_de_thi, d.tong_so_cau, d.thoi_gian_lam_bai, d.trang_thai;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrLine INT = ERROR_LINE();
    DECLARE @ErrNum INT = ERROR_NUMBER();

    RAISERROR(N'[02_seed_exam_a1a_from_pdf.sql] Error %d at line %d: %s', 16, 1, @ErrNum, @ErrLine, @ErrMsg);
END CATCH;
