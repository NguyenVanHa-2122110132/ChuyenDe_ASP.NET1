/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 07/06/2026
    Mô tả    : Trang profile khách hàng - full page
*/
import React from 'react';
import { Link, useNavigate } from 'react-router-dom';

const C = {
    gold: '#b8975a',
    goldLight: '#d4b483',
    cream: '#f7f3ee',
    dark: '#1a1a1a',
    muted: '#8a8178',
    border: '#e4ddd4',
};

export default function ProfilePage() {
    const navigate = useNavigate();
    const fullName = localStorage.getItem('fullName') || '';
    const email = localStorage.getItem('email') || '';
    const initials = fullName.split(' ').map(w => w[0]).slice(-2).join('').toUpperCase();

    const handleLogout = () => {
        localStorage.removeItem('token');
        localStorage.removeItem('fullName');
        localStorage.removeItem('email');
        navigate('/');
        window.location.reload();
    };

    const orderStats = [
        { icon: '🧾', label: 'Chờ xác nhận', to: '/orders?status=pending' },
        { icon: '📦', label: 'Chờ lấy hàng', to: '/orders?status=pickup' },
        { icon: '🚚', label: 'Chờ giao hàng', to: '/orders?status=shipping' },
        { icon: '⭐', label: 'Đánh giá', to: '/orders?status=review' },
    ];

    const utilities = [
        { icon: '❤️', label: 'Yêu thích', sub: 'Sản phẩm đã lưu', to: '/wishlist' },
        { icon: '🛒', label: 'Giỏ hàng', sub: 'Xem sản phẩm đã thêm', to: '/cart' },
        { icon: '⚙️', label: 'Cài đặt tài khoản', sub: 'Thông tin cá nhân', to: '/settings' },
    ];

    return (
        <div style={{ minHeight: '100vh', background: '#f5f5f5', fontFamily: "'Jost',sans-serif" }}>

            {/* ── HEADER ── */}
            <div style={{ background: C.dark, padding: '32px 24px 48px' }}>
                <div style={{ maxWidth: '600px', margin: '0 auto', display: 'flex', alignItems: 'center', gap: '20px' }}>
                    {/* Avatar */}
                    <div style={{ width: '72px', height: '72px', borderRadius: '50%', background: C.gold, display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '1.5rem', fontWeight: 700, color: '#fff', flexShrink: 0, border: '3px solid rgba(255,255,255,0.2)' }}>
                        {initials}
                    </div>
                    <div style={{ flex: 1 }}>
                        <div style={{ color: '#fff', fontWeight: 600, fontSize: '1.1rem', marginBottom: '4px' }}>{fullName}</div>
                        <div style={{ color: C.gold, fontSize: '0.7rem', letterSpacing: '2px', textTransform: 'uppercase', marginBottom: '4px' }}>✦ Thành viên</div>
                        {email && <div style={{ color: '#888', fontSize: '0.75rem' }}>{email}</div>}
                    </div>
                    <Link to="/settings" style={{ color: '#888', fontSize: '1.2rem', textDecoration: 'none' }}>⚙️</Link>
                </div>
            </div>

            {/* ── BODY ── */}
            <div style={{ maxWidth: '600px', margin: '-20px auto 0', padding: '0 16px', position: 'relative', zIndex: 1 }}>

                {/* Đơn mua */}
                <div style={{ background: '#fff', borderRadius: '8px', padding: '20px', marginBottom: '16px', boxShadow: '0 2px 8px rgba(0,0,0,0.06)' }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
                        <span style={{ fontWeight: 600, fontSize: '0.95rem', color: C.dark }}>Đơn mua</span>
                        <Link to="/orders" style={{ fontSize: '0.75rem', color: C.muted, textDecoration: 'none' }}>Xem lịch sử mua hàng ›</Link>
                    </div>
                    <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '8px', textAlign: 'center' }}>
                        {orderStats.map(item => (
                            <Link key={item.label} to={item.to} style={{ textDecoration: 'none', display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '8px', padding: '8px 4px', borderRadius: '6px', transition: 'background 0.15s' }}
                                onMouseEnter={e => e.currentTarget.style.background = C.cream}
                                onMouseLeave={e => e.currentTarget.style.background = 'transparent'}
                            >
                                <span style={{ fontSize: '1.6rem' }}>{item.icon}</span>
                                <span style={{ fontSize: '0.7rem', color: C.muted, lineHeight: 1.3 }}>{item.label}</span>
                            </Link>
                        ))}
                    </div>
                </div>

                {/* Tiện ích */}
                <div style={{ background: '#fff', borderRadius: '8px', marginBottom: '16px', boxShadow: '0 2px 8px rgba(0,0,0,0.06)', overflow: 'hidden' }}>
                    <div style={{ padding: '16px 20px', borderBottom: `1px solid ${C.border}` }}>
                        <span style={{ fontWeight: 600, fontSize: '0.95rem', color: C.dark }}>Tiện ích của tôi</span>
                    </div>
                    {utilities.map((item, i) => (
                        <Link key={item.label} to={item.to}
                            style={{ display: 'flex', alignItems: 'center', gap: '16px', padding: '16px 20px', color: C.dark, textDecoration: 'none', borderBottom: i < utilities.length - 1 ? `1px solid ${C.border}` : 'none', transition: 'background 0.15s' }}
                            onMouseEnter={e => e.currentTarget.style.background = C.cream}
                            onMouseLeave={e => e.currentTarget.style.background = 'transparent'}
                        >
                            <span style={{ fontSize: '1.3rem', width: '32px', textAlign: 'center' }}>{item.icon}</span>
                            <div style={{ flex: 1 }}>
                                <div style={{ fontSize: '0.85rem', fontWeight: 500 }}>{item.label}</div>
                                <div style={{ fontSize: '0.72rem', color: C.muted, marginTop: '2px' }}>{item.sub}</div>
                            </div>
                            <span style={{ color: C.muted }}>›</span>
                        </Link>
                    ))}
                </div>

                {/* FOOTER - Đăng xuất */}
                <div style={{ background: '#fff', borderRadius: '8px', marginBottom: '32px', boxShadow: '0 2px 8px rgba(0,0,0,0.06)' }}>
                    <button onClick={handleLogout}
                        style={{ width: '100%', padding: '16px 20px', background: 'none', border: 'none', color: '#c0392b', fontFamily: "'Jost',sans-serif", fontSize: '0.85rem', cursor: 'pointer', display: 'flex', alignItems: 'center', gap: '12px', transition: 'background 0.15s', borderRadius: '8px' }}
                        onMouseEnter={e => e.currentTarget.style.background = '#fff5f5'}
                        onMouseLeave={e => e.currentTarget.style.background = 'transparent'}
                    >
                        <span style={{ fontSize: '1.1rem' }}>🚪</span>
                        <span style={{ fontWeight: 500 }}>Đăng xuất</span>
                    </button>
                </div>

            </div>
        </div>
    );
}