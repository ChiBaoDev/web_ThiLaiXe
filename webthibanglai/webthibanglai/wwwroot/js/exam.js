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
        submitted: false,
        currentQuestionIndex: 0,
        fullscreenRequested: false
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
        examStatusBadge: document.getElementById('examStatusBadge'),
        answeredCount: document.getElementById('answeredCount'),
        totalCount: document.getElementById('totalCount'),
        progressBar: document.getElementById('progressBar'),
        sidebarCurrentQuestion: document.getElementById('sidebarCurrentQuestion'),
        currentQuestionLabel: document.getElementById('currentQuestionLabel'),
        currentQuestionTitle: document.getElementById('currentQuestionTitle'),
        questionPalette: document.getElementById('questionPalette'),
        questionStage: document.getElementById('questionStage'),
        prevBtn: document.getElementById('prevBtn'),
        nextBtn: document.getElementById('nextBtn'),
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

    function getAnsweredCount() {
        return Object.keys(state.answers).length;
    }

    function updateProgress() {
        const answered = getAnsweredCount();
        const total = state.questions.length;
        const progress = total ? (answered / total) * 100 : 0;

        el.answeredCount.textContent = answered;
        el.totalCount.textContent = total;
        el.progressBar.style.width = `${progress}%`;
        el.progressBar.setAttribute('aria-valuenow', String(Math.round(progress)));

        const currentLabel = `Câu ${state.currentQuestionIndex + 1}`;
        el.sidebarCurrentQuestion.textContent = currentLabel;
        el.currentQuestionLabel.textContent = `${currentLabel} / ${total}`;
        el.currentQuestionTitle.textContent = currentLabel.toLowerCase();
        el.examStatusBadge.textContent = answered === total ? 'Sẵn sàng nộp bài' : 'Đang làm bài';
        el.examStatusBadge.className = answered === total
            ? 'badge rounded-pill bg-success px-3 py-2'
            : 'badge rounded-pill bg-primary px-3 py-2';
    }

    function renderPalette() {
        el.questionPalette.innerHTML = state.questions.map((_, idx) => {
            const isActive = idx === state.currentQuestionIndex;
            const isAnswered = Object.prototype.hasOwnProperty.call(state.answers, idx);
            return `
                <button
                    type="button"
                    class="palette-btn ${isActive ? 'active' : ''} ${isAnswered ? 'answered' : ''}"
                    data-question-index="${idx}"
                    aria-label="Chuyển đến câu ${idx + 1}"
                >${idx + 1}</button>`;
        }).join('');

        el.questionPalette.querySelectorAll('[data-question-index]').forEach(button => {
            button.addEventListener('click', () => {
                goToQuestion(Number(button.dataset.questionIndex));
            });
        });
    }

    function renderCurrentQuestion() {
        const idx = state.currentQuestionIndex;
        const q = state.questions[idx];
        const selected = state.answers[idx];
        const labels = ['A', 'B', 'C', 'D', 'E', 'F'];

        const options = q.options.map((op, opIdx) => `
            <div class="option-item">
                <input
                    class="option-input"
                    type="radio"
                    name="q-${idx}"
                    id="q-${idx}-${opIdx}"
                    value="${opIdx}"
                    ${selected === opIdx ? 'checked' : ''}
                >
                <label class="option-label" for="q-${idx}-${opIdx}">
                    <span class="option-marker">${labels[opIdx] || opIdx + 1}</span>
                    <span class="option-copy">${op}</span>
                </label>
            </div>`).join('');

        el.questionStage.innerHTML = `
            <div>
                <span class="question-tag">Câu hỏi ${idx + 1}/${state.questions.length}</span>
                <div class="question-text">${q.question}</div>
                <div class="option-list">${options}</div>
            </div>
            <div class="text-muted mt-4">Mã câu hỏi: ${q.id}</div>`;

        el.questionStage.querySelectorAll('input[type="radio"]').forEach(input => {
            input.addEventListener('change', (e) => {
                state.answers[idx] = Number(e.target.value);
                updateProgress();
                renderPalette();
                renderCurrentQuestion();
            });
        });

        el.prevBtn.disabled = idx === 0;
        el.nextBtn.innerHTML = idx === state.questions.length - 1
            ? 'Hoàn tất câu cuối<i class="fas fa-check ms-2"></i>'
            : 'Câu tiếp theo<i class="fas fa-arrow-right ms-2"></i>';

        updateProgress();
        renderPalette();
    }

    function goToQuestion(index) {
        if (index < 0 || index >= state.questions.length) return;
        state.currentQuestionIndex = index;
        renderCurrentQuestion();
    }

    async function requestExamFullscreen() {
        if (state.fullscreenRequested || !document.documentElement.requestFullscreen) return;
        try {
            await document.documentElement.requestFullscreen();
            state.fullscreenRequested = true;
            document.body.classList.add('exam-fullscreen-active');
        } catch (error) {
            document.body.classList.add('exam-fullscreen-active');
        }
    }

    function syncFullscreenState() {
        const active = Boolean(document.fullscreenElement);
        document.body.classList.toggle('exam-fullscreen-active', active || state.submitted === false && !el.examPanel.classList.contains('d-none'));
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
        el.examStatusBadge.textContent = pass ? 'Đã hoàn thành' : 'Đã nộp bài';
        el.examStatusBadge.className = pass
            ? 'badge rounded-pill bg-success px-3 py-2'
            : 'badge rounded-pill bg-danger px-3 py-2';

        if (document.fullscreenElement && document.exitFullscreen) {
            document.exitFullscreen().catch(() => {});
        }
        document.body.classList.remove('exam-fullscreen-active');
        window.scrollTo({ top: document.body.scrollHeight, behavior: 'smooth' });
    }

    async function startExam() {
        state.type = el.type.value;
        state.submitted = false;
        state.answers = {};
        state.questions = pickQuestions(state.type);
        state.remainSeconds = EXAM_CONFIG[state.type].durationMinutes * 60;
        state.currentQuestionIndex = 0;
        state.fullscreenRequested = false;

        el.examTypeLabel.textContent = state.type;
        el.examPanel.classList.remove('d-none');
        el.resultPanel.classList.add('d-none');

        updateProgress();
        renderCurrentQuestion();
        startTimer();
        await requestExamFullscreen();
        syncFullscreenState();
        window.scrollTo({ top: el.examPanel.offsetTop - 24, behavior: 'smooth' });
    }

    el.type.addEventListener('change', renderConfig);
    el.startBtn.addEventListener('click', startExam);
    el.submitBtn.addEventListener('click', () => submitExam(false));
    el.prevBtn.addEventListener('click', () => goToQuestion(state.currentQuestionIndex - 1));
    el.nextBtn.addEventListener('click', () => {
        if (state.currentQuestionIndex === state.questions.length - 1) {
            submitExam(false);
            return;
        }
        goToQuestion(state.currentQuestionIndex + 1);
    });
    document.addEventListener('fullscreenchange', syncFullscreenState);

    renderConfig();
})();