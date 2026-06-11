/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 05/06/2026
    Mô tả    : Trang đăng ký - React (có xác thực OTP qua email)
               Bước 1: Nhập thông tin → gửi OTP về email
               Bước 2: Nhập OTP → tạo tài khoản thành công
*/
import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';

const C = {
    gold: '#b8975a',
    goldLight: '#d4b483',
    cream: '#f7f3ee',
    dark: '#1a1a1a',
    muted: '#8a8178',
    border: '#e4ddd4',
};

export default function RegisterPage() {
    const navigate = useNavigate();

    // ── State bước đăng ký ──
    const [step, setStep] = useState(1); // 1: nhập thông tin, 2: nhập OTP

    // ── State form bước 1 ──
    const [form, setForm] = useState({
        fullName: '',
        email: '',
        password: '',
        confirmPassword: ''
    });

    // ── State bước 2 ──
    const [otp, setOtp] = useState('');

    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');
    const [loading, setLoading] = useState(false);

    const handleChange = e => setForm({ ...form, [e.target.name]: e.target.value });

    // ========== BƯỚC 1: GỬI OTP ==========
    const handleSendOtp = async e => {
        e.preventDefault();
        setError('');

        if (form.password !== form.confirmPassword) {
            setError('Mật khẩu xác nhận không khớp.');
            return;
        }

        setLoading(true);
        try {
            const res = await fetch('https://localhost:7038/api/auth/send-otp', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    fullName: form.fullName,
                    email: form.email,
                }),
            });

            const data = await res.json();

            if (!res.ok) {
                setError(data.message || 'Gửi OTP thất bại. Vui lòng thử lại.');
            } else {
                setSuccess(`Mã OTP đã được gửi về ${form.email}. Vui lòng kiểm tra hòm thư!`);
                setStep(2); // Chuyển sang bước 2
            }
        } catch {
            setError('Không thể kết nối đến máy chủ.');
        } finally {
            setLoading(false);
        }
    };

    // ========== BƯỚC 2: XÁC NHẬN OTP ==========
    const handleVerifyOtp = async e => {
        e.preventDefault();
        setError('');
        setLoading(true);

        try {
            const res = await fetch('https://localhost:7038/api/auth/verify-otp', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    fullName: form.fullName,
                    email: form.email,
                    otp: otp,
                    password: form.password,
                }),
            });

            const data = await res.json();

            if (!res.ok) {
                setError(data.message || 'Mã OTP không hợp lệ.');
            } else {
                setSuccess('Đăng ký thành công! Đang chuyển đến trang đăng nhập...');
                setTimeout(() => navigate('/login'), 2000);
            }
        } catch {
            setError('Không thể kết nối đến máy chủ.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <>
            <style>{`
                @import url('https://fonts.googleapis.com/css2?family=Cormorant+Garamond:ital,wght@0,300;0,400;0,600;1,300;1,400&family=Jost:wght@200;300;400;500&display=swap');
                .rp-page { min-height:100vh; display:grid; grid-template-columns:1fr 1fr; font-family:'Jost',sans-serif; overflow:hidden; }
                .rp-left { position:relative; overflow:hidden; background:#1a1a1a; }
                .rp-left img { width:100%; height:100%; object-fit:cover; opacity:0.55; }
                .rp-overlay { position:absolute; inset:0; background:linear-gradient(135deg,rgba(26,26,26,0.7) 0%,rgba(184,151,90,0.15) 100%); display:flex; flex-direction:column; justify-content:flex-end; padding:56px; }
                .rp-brand { position:absolute; top:48px; left:56px; }
                .rp-brand h1 { font-family:'Cormorant Garamond',serif; font-size:2rem; font-weight:300; color:#fff; letter-spacing:4px; text-transform:uppercase; line-height:1; }
                .rp-brand p { font-size:0.6rem; letter-spacing:5px; color:#b8975a; text-transform:uppercase; margin-top:4px; }
                .rp-right { display:flex; flex-direction:column; justify-content:center; align-items:center; padding:40px 72px; background:#f7f3ee; position:relative; overflow-y:auto; }
                .rp-right::before { content:''; position:absolute; top:0; left:0; right:0; height:3px; background:linear-gradient(90deg,#b8975a,#d4b483,#b8975a); }
                .rp-box { width:100%; max-width:360px; }
                .rp-field { margin-bottom:18px; }
                .rp-field label { display:block; font-size:0.62rem; letter-spacing:2.5px; color:#8a8178; text-transform:uppercase; margin-bottom:8px; }
                .rp-field input { width:100%; padding:14px 18px; background:#fff; border:1px solid #e4ddd4; color:#1a1a1a; font-family:'Jost',sans-serif; font-size:0.9rem; outline:none; transition:border-color 0.25s; box-sizing:border-box; }
                .rp-field input:focus { border-color:#b8975a; box-shadow:0 0 0 3px rgba(184,151,90,0.08); }
                .rp-field input::placeholder { color:#c5bdb5; }
                .rp-error { background:#fff5f5; border-left:3px solid #c0392b; padding:12px 16px; color:#c0392b; font-size:0.78rem; margin-bottom:20px; }
                .rp-success { background:#f0fff4; border-left:3px solid #27ae60; padding:12px 16px; color:#27ae60; font-size:0.78rem; margin-bottom:20px; }
                .rp-btn { width:100%; padding:16px; background:#1a1a1a; color:#fff; border:none; font-family:'Jost',sans-serif; font-size:0.72rem; font-weight:500; letter-spacing:3px; text-transform:uppercase; cursor:pointer; transition:background 0.25s; }
                .rp-btn:hover { background:#111; }
                .rp-btn:disabled { opacity:0.6; cursor:not-allowed; }
                .rp-step { display:flex; gap:8px; margin-bottom:28px; }
                .rp-step-item { flex:1; height:3px; border-radius:2px; transition:background 0.3s; }
                .rp-footer { margin-top:28px; padding-top:20px; border-top:1px solid #e4ddd4; text-align:center; }
                .rp-footer p { font-size:0.65rem; letter-spacing:1.5px; color:#8a8178; text-transform:uppercase; }
                .rp-footer a { color:#b8975a; text-decoration:none; font-weight:500; }
                .otp-input { width:100%; padding:20px; background:#fff; border:2px solid #e4ddd4; color:#1a1a1a; font-family:'Jost',sans-serif; font-size:2rem; font-weight:600; letter-spacing:12px; text-align:center; outline:none; transition:border-color 0.25s; box-sizing:border-box; }
                .otp-input:focus { border-color:#b8975a; }
                @media(max-width:768px){ .rp-page{grid-template-columns:1fr} .rp-left{display:none} }
            `}</style>

            <div className="rp-page">
                {/* LEFT */}
                <div className="rp-left">
                    <img src="https://images.unsplash.com/photo-1490481651871-ab68de25d43d?q=80&w=900" alt="Fashion" />
                    <div className="rp-overlay">
                        <div className="rp-brand">
                            <h1>Mai Trinh</h1>
                            <p>Studio</p>
                        </div>
                        <div>
                            <span style={{ fontSize: '0.62rem', letterSpacing: '4px', color: '#b8975a', textTransform: 'uppercase', display: 'block', marginBottom: '14px' }}>
                                Chào Mừng Thành Viên Mới
                            </span>
                            <h2 style={{ fontFamily: "'Cormorant Garamond',serif", fontSize: '2.6rem', fontWeight: 300, color: '#fff', lineHeight: 1.25, margin: 0 }}>
                                Phong Cách<br /><em style={{ color: '#d4b483' }}>Bắt Đầu</em><br />Từ Đây
                            </h2>
                        </div>
                    </div>
                </div>

                {/* RIGHT */}
                <div className="rp-right">
                    <div className="rp-box">

                        {/* Header */}
                        <div style={{ marginBottom: '28px' }}>
                            <p style={{ fontSize: '0.62rem', letterSpacing: '4px', color: C.gold, textTransform: 'uppercase', marginBottom: '10px' }}>
                                Mai Trinh Studio
                            </p>
                            <h3 style={{ fontFamily: "'Cormorant Garamond',serif", fontSize: '2.2rem', fontWeight: 300, color: C.dark, margin: 0 }}>
                                {step === 1 ? <>Đăng <em style={{ color: C.gold }}>Ký</em></> : <>Xác <em style={{ color: C.gold }}>Nhận</em></>}
                            </h3>
                        </div>

                        {/* Step indicator */}
                        <div className="rp-step">
                            <div className="rp-step-item" style={{ background: C.gold }} />
                            <div className="rp-step-item" style={{ background: step === 2 ? C.gold : C.border }} />
                        </div>

                        {error && <div className="rp-error">⚠ {error}</div>}
                        {success && <div className="rp-success">✓ {success}</div>}

                        {/* ── BƯỚC 1: NHẬP THÔNG TIN ── */}
                        {step === 1 && (
                            <form onSubmit={handleSendOtp}>
                                <div className="rp-field">
                                    <label>Họ và tên</label>
                                    <input type="text" name="fullName" placeholder="Nhập họ tên..." required value={form.fullName} onChange={handleChange} />
                                </div>
                                <div className="rp-field">
                                    <label>Email</label>
                                    <input type="email" name="email" placeholder="Nhập email..." required value={form.email} onChange={handleChange} />
                                </div>
                                <div className="rp-field">
                                    <label>Mật khẩu</label>
                                    <input type="password" name="password" placeholder="Nhập mật khẩu..." required value={form.password} onChange={handleChange} />
                                </div>
                                <div className="rp-field">
                                    <label>Xác nhận mật khẩu</label>
                                    <input type="password" name="confirmPassword" placeholder="Nhập lại mật khẩu..." required value={form.confirmPassword} onChange={handleChange} />
                                </div>
                                <button type="submit" className="rp-btn" disabled={loading}>
                                    {loading ? 'Đang gửi OTP...' : 'Tiếp Theo →'}
                                </button>
                            </form>
                        )}

                        {/* ── BƯỚC 2: NHẬP OTP ── */}
                        {step === 2 && (
                            <form onSubmit={handleVerifyOtp}>
                                <div style={{ textAlign: 'center', marginBottom: '24px' }}>
                                    <p style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.8rem', color: C.muted, lineHeight: 1.6 }}>
                                        Mã OTP đã được gửi về<br />
                                        <strong style={{ color: C.dark }}>{form.email}</strong>
                                    </p>
                                </div>
                                <div className="rp-field">
                                    <label>Nhập mã OTP (6 số)</label>
                                    <input
                                        className="otp-input"
                                        type="text"
                                        maxLength={6}
                                        placeholder="000000"
                                        value={otp}
                                        onChange={e => setOtp(e.target.value.replace(/\D/g, ''))}
                                        required
                                    />
                                </div>
                                <button type="submit" className="rp-btn" disabled={loading || otp.length !== 6}>
                                    {loading ? 'Đang xác nhận...' : 'Xác Nhận & Đăng Ký'}
                                </button>
                                <button
                                    type="button"
                                    onClick={() => { setStep(1); setError(''); setSuccess(''); setOtp(''); }}
                                    style={{ width: '100%', marginTop: '12px', padding: '12px', background: 'none', border: `1px solid ${C.border}`, color: C.muted, fontFamily: "'Jost',sans-serif", fontSize: '0.72rem', letterSpacing: '2px', cursor: 'pointer', textTransform: 'uppercase' }}
                                >
                                    ← Quay lại
                                </button>
                            </form>
                        )}

                        <div className="rp-footer">
                            <p>Đã có tài khoản? <Link to="/login">Đăng nhập</Link></p>
                        </div>
                    </div>
                </div>
            </div>
        </>
    );
}