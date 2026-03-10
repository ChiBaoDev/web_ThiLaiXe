(function () {
    const EXAM_CONFIG = {
        A1: { totalQuestions: 25, durationMinutes: 19, passScore: 21 },
        A: { totalQuestions: 25, durationMinutes: 19, passScore: 21 }
    };

    const QUESTION_BANK = {
        A1: Array.from({ length: 30 }, (_, i) => ({
            id: `A1-${String(i + 1).padStart(3, '0')}`,
            question: `Câu ${i + 1}: Khi tham gia giao thông, người lái xe máy cần ưu tiên điều gì?`,
            options: [
                'Đi nhanh để vượt đèn vàng',
                'Tuân thủ luật và quan sát an toàn',
                'Bấm còi liên tục để xin đường',
                'Đi sát xe phía trước'
            ],
            correctIndex: 1,
            explanation: 'Người lái xe phải tuân thủ luật giao thông và quan sát an toàn.'
        })),
        A: Array.from({ length: 30 }, (_, i) => ({
            id: `A-${String(i + 1).padStart(3, '0')}`,
            question: `Câu ${i + 1}: Quy tắc an toàn nào quan trọng khi lái xe mô tô hạng A?`,
            options: [
                'Luôn giữ khoảng cách an toàn',
                'Tăng ga liên tục khi đông xe',
                'Đi vào làn ô tô khi đường trống',
                'Chỉ cần đội mũ khi đi xa'
            ],
            correctIndex: 0,
            explanation: 'Giữ khoảng cách an toàn là nguyên tắc cốt lõi để giảm tai nạn.'
        }))
    };

    const state = {
        type: 'A1',
        questions: [],
        answers: {},
        remainSeconds: 0,
        timer: null,
        submitted: false
    };

    const el = {
        type: document.getElementById('licenseType'),
        cfgQuestions: document.getElementById('cfgQuestions'),
        cfgTime: document.getElementById('cfgTime'),
        cfgPass: document.getElementById('cfgPass'),
        startBtn: document.getElementById('startBtn'),
        setupPanel: document.getElementById('setupPanel'),
        examPanel: document.getElementById('examPanel'),
        resultPanel: document.getElementById('resultPanel'),
        examTypeLabel: document.getElementById('examTypeLabel'),
        timer: document.getElementById('timer'),
        form: document.getElementById('examForm'),
        submitBtn: document.getElementById('submitBtn'),
        rTotal: document.getElementById('rTotal'),
        rCorrect: document.getElementById('rCorrect'),
        rWrong: document.getElementById('rWrong'),
        rStatus: document.getElementById('rStatus'),
        wrongList: document.getElementById('wrongList')
    };

    function shuffle(arr) {
        const a = [...arr];
        for (let i = a.length - 1; i > 0; i--) {
            const j = Math.floor(Math.random() * (i + 1));
            [a[i], a[j]] = [a[j], a[i]];
        }
        return a;
    }

    function pickQuestions(type) {
        const cfg = EXAM_CONFIG[type];
        return shuffle(QUESTION_BANK[type]).slice(0, cfg.totalQuestions);
    }

    function fmtTime(sec) {
        const m = Math.floor(sec / 60).toString().padStart(2, '0');
        const s = (sec % 60).toString().padStart(2, '0');
        return `${m}:${s}`;
    }

    function renderConfig() {
        const type = el.type.value;
        const cfg = EXAM_CONFIG[type];
        el.cfgQuestions.textContent = cfg.totalQuestions;
        el.cfgTime.textContent = cfg.durationMinutes;
        el.cfgPass.textContent = cfg.passScore;
    }

    function renderQuestions() {
        el.form.innerHTML = state.questions.map((q, idx) => {
            const opts = q.options.map((op, opIdx) => `
                <div class="form-check mb-1">
                    <input class="form-check-input" type="radio" name="q-${idx}" id="q-${idx}-${opIdx}" value="${opIdx}">
                    <label class="form-check-label" for="q-${idx}-${opIdx}">${op}</label>
                </div>`).join('');
            return `
                <div class="card border-0 shadow-sm mb-3">
                    <div class="card-body">
                        <h6 class="mb-3">${idx + 1}. ${q.question}</h6>
                        ${opts}
                    </div>
                </div>`;
        }).join('');

        el.form.querySelectorAll('input[type="radio"]').forEach(input => {
            input.addEventListener('change', (e) => {
                const [_, qIndex] = e.target.name.split('-');
                state.answers[qIndex] = Number(e.target.value);
            });
        });
    }

    function startTimer() {
        clearInterval(state.timer);
        el.timer.textContent = fmtTime(state.remainSeconds);
        state.timer = setInterval(() => {
            state.remainSeconds--;
            el.timer.textContent = fmtTime(Math.max(0, state.remainSeconds));
            if (state.remainSeconds <= 0) {
                submitExam(true);
            }
        }, 1000);
    }

    function submitExam(auto = false) {
        if (state.submitted) return;
        state.submitted = true;
        clearInterval(state.timer);

        let correct = 0;
        const wrongItems = [];

        state.questions.forEach((q, idx) => {
            const picked = state.answers[idx];
            if (picked === q.correctIndex) {
                correct++;
            } else {
                wrongItems.push({
                    index: idx + 1,
                    question: q.question,
                    correctAnswer: q.options[q.correctIndex],
                    yourAnswer: typeof picked === 'number' ? q.options[picked] : 'Chưa trả lời',
                    explanation: q.explanation
                });
            }
        });

        const cfg = EXAM_CONFIG[state.type];
        const pass = correct >= cfg.passScore;

        el.rTotal.textContent = state.questions.length;
        el.rCorrect.textContent = correct;
        el.rWrong.textContent = state.questions.length - correct;
        el.rStatus.textContent = pass ? 'ĐẠT' : 'CHƯA ĐẠT';
        el.rStatus.className = pass ? 'text-success' : 'text-danger';

        if (wrongItems.length === 0) {
            el.wrongList.innerHTML = '<p class="text-success mb-0">Bạn đã trả lời đúng toàn bộ câu hỏi.</p>';
        } else {
            el.wrongList.innerHTML = wrongItems.map(w => `
                <div class="border rounded p-3 mb-2">
                    <div><strong>Câu ${w.index}:</strong> ${w.question}</div>
                    <div><strong>Bạn chọn:</strong> ${w.yourAnswer}</div>
                    <div><strong>Đáp án đúng:</strong> ${w.correctAnswer}</div>
                    <div class="text-muted"><em>Giải thích:</em> ${w.explanation}</div>
                </div>
            `).join('');
        }

        const payload = {
            type: state.type,
            total: state.questions.length,
            correct,
            wrong: state.questions.length - correct,
            pass,
            autoSubmit: auto,
            timeLeft: state.remainSeconds,
            generatedAt: new Date().toISOString()
        };
        localStorage.setItem('thiXeMayLastResult', JSON.stringify(payload));

        el.resultPanel.classList.remove('d-none');
        window.scrollTo({ top: document.body.scrollHeight, behavior: 'smooth' });
    }

    function startExam() {
        state.type = el.type.value;
        state.submitted = false;
        state.answers = {};
        state.questions = pickQuestions(state.type);
        state.remainSeconds = EXAM_CONFIG[state.type].durationMinutes * 60;

        el.examTypeLabel.textContent = state.type;
        el.examPanel.classList.remove('d-none');
        el.resultPanel.classList.add('d-none');

        renderQuestions();
        startTimer();
    }

    el.type.addEventListener('change', renderConfig);
    el.startBtn.addEventListener('click', startExam);
    el.submitBtn.addEventListener('click', () => submitExam(false));

    renderConfig();
})();