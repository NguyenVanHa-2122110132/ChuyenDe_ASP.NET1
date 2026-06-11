/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 04/06/2026
    Mô tả    : Trang đăng nhập - React (phong cách thời trang cao cấp)
               File: src/pages/LoginPage.jsx
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

export default function LoginPage() {
    const navigate = useNavigate();
    const [form, setForm] = useState({ email: '', password: '' });
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    const handleChange = e => setForm({ ...form, [e.target.name]: e.target.value });

    const handleSubmit = async e => {
        e.preventDefault();
        setError('');
        setLoading(true);
        try {
            const res = await fetch('https://localhost:7038/api/auth/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email: form.email, password: form.password })
            });
            if (!res.ok) {
                const data = await res.json();
                setError(data.message || 'Tên đăng nhập hoặc mật khẩu không đúng.');
            } else {
                const data = await res.json();
                localStorage.setItem('token', data.token);
                localStorage.setItem('fullName', data.fullName);
                navigate('/');
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
                .login-page { min-height:100vh; display:grid; grid-template-columns:1fr 1fr; font-family:'Jost',sans-serif; overflow:hidden; }
                .lp-left { position:relative; overflow:hidden; background:${C.dark}; }
                .lp-left img { width:100%; height:100%; object-fit:cover; opacity:0.55; transform:scale(1.05); transition:transform 8s ease; }
                .lp-left:hover img { transform:scale(1.0); }
                .lp-overlay { position:absolute; inset:0; background:linear-gradient(135deg,rgba(26,26,26,0.7) 0%,rgba(184,151,90,0.15) 100%); display:flex; flex-direction:column; justify-content:flex-end; padding:56px; }
                .lp-brand { position:absolute; top:48px; left:56px; }
                .lp-brand h1 { font-family:'Cormorant Garamond',serif; font-size:2rem; font-weight:300; color:#fff; letter-spacing:4px; text-transform:uppercase; line-height:1; }
                .lp-brand p { font-size:0.6rem; letter-spacing:5px; color:${C.gold}; text-transform:uppercase; margin-top:4px; font-weight:300; }
                .lp-quote span { display:block; font-size:0.62rem; letter-spacing:4px; color:${C.gold}; text-transform:uppercase; margin-bottom:14px; font-weight:400; }
                .lp-quote h2 { font-family:'Cormorant Garamond',serif; font-size:2.6rem; font-weight:300; line-height:1.25; color:#fff; }
                .lp-quote h2 em { font-style:italic; color:${C.goldLight}; }
                .lp-deco { width:48px; height:1px; background:${C.gold}; margin:24px 0; opacity:0.7; }
                .lp-right { display:flex; flex-direction:column; justify-content:center; align-items:center; padding:60px 72px; background:${C.cream}; position:relative; }
                .lp-right::before { content:''; position:absolute; top:0; left:0; right:0; height:3px; background:linear-gradient(90deg,${C.gold},${C.goldLight},${C.gold}); }
                .lp-box { width:100%; max-width:360px; animation:fadeUp 0.6s ease both; }
                .lp-box-header { margin-bottom:40px; }
                .lp-box-header p { font-size:0.62rem; letter-spacing:4px; color:${C.gold}; text-transform:uppercase; margin-bottom:10px; font-weight:400; }
                .lp-box-header h3 { font-family:'Cormorant Garamond',serif; font-size:2.2rem; font-weight:300; color:${C.dark}; line-height:1.2; }
                .lp-box-header h3 em { font-style:italic; color:${C.gold}; }
                .lp-field { margin-bottom:20px; }
                .lp-field label { display:block; font-size:0.62rem; letter-spacing:2.5px; color:${C.muted}; text-transform:uppercase; margin-bottom:8px; font-weight:400; }
                .lp-field input { width:100%; padding:14px 18px; background:#fff; border:1px solid ${C.border}; color:${C.dark}; font-family:'Jost',sans-serif; font-size:0.9rem; font-weight:300; outline:none; transition:border-color 0.25s,box-shadow 0.25s; border-radius:0; letter-spacing:0.5px; }
                .lp-field input:focus { border-color:${C.gold}; box-shadow:0 0 0 3px rgba(184,151,90,0.08); }
                .lp-field input::placeholder { color:#c5bdb5; font-weight:300; }
                .lp-error { background:#fff5f5; border-left:3px solid #c0392b; padding:12px 16px; color:#c0392b; font-size:0.78rem; margin-bottom:20px; font-weight:300; }
                .lp-forgot { display:block; text-align:right; font-size:0.7rem; letter-spacing:1px; color:${C.muted}; text-decoration:none; margin-bottom:28px; transition:color 0.2s; }
                .lp-forgot:hover { color:${C.gold}; }
                .lp-btn { width:100%; padding:16px; background:${C.dark}; color:#fff; border:none; font-family:'Jost',sans-serif; font-size:0.72rem; font-weight:500; letter-spacing:3px; text-transform:uppercase; cursor:pointer; transition:background 0.25s,transform 0.15s; }
                .lp-btn:hover { background:#111; transform:translateY(-1px); }
                .lp-btn:active { transform:translateY(0); }
                .lp-btn:disabled { opacity:0.6; cursor:not-allowed; }
                .lp-accent { height:2px; background:linear-gradient(90deg,${C.gold},${C.goldLight}); transform:scaleX(0); transform-origin:left; transition:transform 0.3s ease; }
                .lp-btn:hover + .lp-accent { transform:scaleX(1); }
                .lp-footer { margin-top:32px; padding-top:24px; border-top:1px solid ${C.border}; text-align:center; }
                .lp-footer p { font-size:0.65rem; letter-spacing:1.5px; color:${C.muted}; text-transform:uppercase; font-weight:300; }
                .lp-footer a { color:${C.gold}; text-decoration:none; font-weight:500; }
                .lp-footer a:hover { text-decoration:underline; }
                .lp-corner-br { position:absolute; bottom:40px; right:40px; width:60px; height:60px; border-right:1px solid ${C.border}; border-bottom:1px solid ${C.border}; opacity:0.5; }
                .lp-corner-tl { position:absolute; top:40px; left:40px; width:60px; height:60px; border-left:1px solid ${C.border}; border-top:1px solid ${C.border}; opacity:0.5; }
                @keyframes fadeUp { from{opacity:0;transform:translateY(20px)} to{opacity:1;transform:translateY(0)} }
                @media(max-width:768px){ .login-page{grid-template-columns:1fr} .lp-left{display:none} }
            `}</style>

            <div className="login-page">
                {/* LEFT */}
                <div className="lp-left">
                    <img src="https://images.unsplash.com/photo-1469334031218-e382a71b716b?q=80&w=900" alt="Fashion" />
                    <div className="lp-overlay">
                        <div className="lp-brand">
                            <h1>Mai Trinh</h1>
                            <p>Studio</p>
                        </div>
                        <div className="lp-quote">
                            <span>Bộ Sưu Tập 2026</span>
                            <h2>Thời Trang<br /><em>Tinh Tế &</em><br />Sang Trọng</h2>
                            <div className="lp-deco"></div>
                            <p style={{ fontSize: '0.75rem', color: 'rgba(255,255,255,0.5)', fontWeight: 300, letterSpacing: '1px', lineHeight: 1.6 }}>
                                Khám phá phong cách thời trang<br />cao cấp dành riêng cho bạn
                            </p>
                        </div>
                    </div>
                </div>

                {/* RIGHT */}
                <div className="lp-right">
                    <div className="lp-corner-tl"></div>
                    <div className="lp-corner-br"></div>

                    <div className="lp-box">
                        <div className="lp-box-header">
                            <p>Mai Trinh Studio</p>
                            <h3>Đăng <em>Nhập</em></h3>
                        </div>

                        {error && <div className="lp-error">⚠ {error}</div>}

                        <form onSubmit={handleSubmit}>
                            <div className="lp-field">
                                <label>Email</label>
                                <input type="email" name="email" placeholder="Nhập email..." autoComplete="off" required value={form.email} onChange={handleChange} />
                            </div>
                            <div className="lp-field">
                                <label>Mật khẩu</label>
                                <input type="password" name="password" placeholder="Nhập mật khẩu..." autoComplete="off" required value={form.password} onChange={handleChange} />
                            </div>

                            <a href="#" className="lp-forgot">Quên mật khẩu?</a>

                            <button type="submit" className="lp-btn" disabled={loading}>
                                {loading ? 'Đang đăng nhập...' : 'Đăng Nhập'}
                            </button>
                            <div className="lp-accent"></div>
                        </form>

                        <div className="lp-footer">
                            <p>Chưa có tài khoản? <Link to="/register">Đăng ký ngay</Link></p>
                        </div>
                    </div>
                </div>
            </div>
        </>
    );
}
