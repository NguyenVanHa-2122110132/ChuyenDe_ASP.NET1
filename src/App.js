/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : App.js - Thêm React Router, THỜI TRANG → /thoi-trang
               Xoá mega menu dropdown, thay bằng Link navigate
*/
import React, { useState, useEffect } from 'react';
import { Link, Routes, Route, useNavigate } from 'react-router-dom';
import CategoryProductList from './components/CategoryProductList';
import ProductList from './components/ProductList';
import BlogCategoryList from './components/BlogCategoryList';
import FashionPage from './pages/FashionPage';
import Shop from './pages/Shop';
import './App.css';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import ProfilePage from './pages/ProfilePage';
import CartPage from './pages/Cart';
import CheckoutPage from './pages/Checkout';
const C = {
    gold: '#b8975a',
    goldLight: '#d4b483',
    cream: '#f7f3ee',
    creamDark: '#ede8e0',
    white: '#ffffff',
    dark: '#1a1a1a',
    muted: '#8a8178',
    border: '#e4ddd4',
};

const GENDER_LABELS = { nam: '👔 Nam', nu: '👗 Nữ', treem: '🧒 Trẻ Em' };


   //HEADER (dùng chung cho mọi trang)

function Header() {
    const [cartCount, setCartCount] = useState(0);
    useEffect(() => {
        const updateCount = () => {
            const cart = JSON.parse(localStorage.getItem('cart') || '[]');
            setCartCount(cart.reduce((sum, i) => sum + i.quantity, 0));
        };
        updateCount();
        window.addEventListener('cartUpdated', updateCount);
        return () => window.removeEventListener('cartUpdated', updateCount);
    }, []);
    const navigate = useNavigate();
    const NAV_LINKS = [
        { label: 'NƯỚC HOA', to: '/nuoc-hoa' },
        { label: 'MỸ PHẨM', to: '/my-pham' },
        { label: 'PHỤ KIỆN', to: '/phu-kien' },
        { label: 'XU HƯỚNG', to: '/blog' },
        { label: 'LIÊN HỆ', to: '#' },
    ];

    const navStyle = {
        fontFamily: "'Jost',sans-serif",
        fontSize: '0.72rem',
        letterSpacing: '1.5px',
        color: C.dark,
        textDecoration: 'none',
        fontWeight: 500,
        background: 'none',
        border: 'none',
        cursor: 'pointer',
        padding: 0,
    };
    // Đọc trạng thái đăng nhập từ localStorage
    const token = localStorage.getItem('token');
    const fullName = localStorage.getItem('fullName');
    const isLoggedIn = !!token;

    const handleLogout = () => {
        localStorage.removeItem('token');
        localStorage.removeItem('fullName');
        navigate('/');
        window.location.reload();
    };

    return (
        <header style={{ backgroundColor: C.white, borderBottom: `1px solid ${C.border}`, position: 'sticky', top: 0, zIndex: 200 }}>
            <div style={{ maxWidth: '1200px', margin: '0 auto', padding: '0 24px', display: 'flex', alignItems: 'center', justifyContent: 'space-between', height: '64px' }}>

                {/* Logo */}
                <Link to="/" style={{ textDecoration: 'none', display: 'flex', flexDirection: 'column', lineHeight: 1.1 }}>
                    <span style={{ fontSize: '1.3rem', fontWeight: 600, letterSpacing: '3px', color: C.dark, textTransform: 'uppercase' }}>MAI TRINH</span>
                    <span style={{ fontSize: '0.65rem', letterSpacing: '5px', color: C.gold, textTransform: 'uppercase', fontFamily: "'Jost',sans-serif", fontWeight: 400 }}>STUDIO</span>
                </Link>

                {/* Nav */}
                <nav style={{ display: 'flex', gap: '32px', alignItems: 'center', height: '100%' }}>
                    <Link to="/" style={navStyle}
                        onMouseEnter={e => e.target.style.color = C.gold}
                        onMouseLeave={e => e.target.style.color = C.dark}>
                        TRANG CHỦ
                    </Link>

                    {/* THỜI TRANG → /thoi-trang (không còn dropdown) */}
                    <Link
                        to="/thoi-trang"
                        style={{ ...navStyle, color: C.dark }}
                        onMouseEnter={e => e.target.style.color = C.gold}
                        onMouseLeave={e => e.target.style.color = C.dark}
                    >
                        THỜI TRANG
                    </Link>

                    {NAV_LINKS.map(item => (
                        <Link key={item.label} to={item.to} style={navStyle}
                            onMouseEnter={e => e.target.style.color = C.gold}
                            onMouseLeave={e => e.target.style.color = C.dark}>
                            {item.label}
                        </Link>
                    ))}
                </nav>

                {/* Icons */}
                <div style={{ display: 'flex', gap: '20px', alignItems: 'center' }}>
                    {/* Login / Register hoặc Xin chào */}
                    {/* Icons */}
                    <div style={{ display: 'flex', gap: '20px', alignItems: 'center', position: 'relative' }}>
                        {isLoggedIn ? (
                            <Link to="/profile" style={{ textDecoration: 'none' }}>
                                <div style={{ width: '36px', height: '36px', borderRadius: '50%', border: `2px solid ${C.gold}`, display: 'flex', alignItems: 'center', justifyContent: 'center', cursor: 'pointer', transition: 'all 0.2s' }}
                                    onMouseEnter={e => e.currentTarget.style.background = C.gold}
                                    onMouseLeave={e => e.currentTarget.style.background = 'transparent'}
                                >
                                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke={C.gold} strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
                                        <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" />
                                        <circle cx="12" cy="7" r="4" />
                                    </svg>
                                </div>
                            </Link>
                        ) : (
                            <div style={{ display: 'flex', gap: '6px', alignItems: 'center', fontFamily: "'Jost',sans-serif", fontSize: '0.72rem', letterSpacing: '1px' }}>
                                <Link to="/login" style={{ color: C.dark, textDecoration: 'none', fontWeight: 500 }}
                                    onMouseEnter={e => e.target.style.color = C.gold}
                                    onMouseLeave={e => e.target.style.color = C.dark}>
                                    Đăng nhập
                                </Link>
                                <span style={{ color: C.muted }}>/</span>
                                <Link to="/register" style={{ color: C.dark, textDecoration: 'none', fontWeight: 500 }}
                                    onMouseEnter={e => e.target.style.color = C.gold}
                                    onMouseLeave={e => e.target.style.color = C.dark}>
                                    Đăng ký
                                </Link>
                            </div>
                        )}

                        {/* Cart */}
                        <Link to="/cart" style={{ color: C.dark, fontSize: '1rem', position: 'relative' }}>
                            <i className="fas fa-shopping-bag"></i>
                            {cartCount > 0 && (
                                <span style={{ position: 'absolute', top: '-8px', right: '-8px', background: C.gold, color: '#fff', borderRadius: '50%', width: '16px', height: '16px', fontSize: '10px', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>{cartCount}</span>
                            )}
                        </Link>
                    </div>

                </div>
            </div>
        </header>
    );
}


   //TRANG CHỦ (HomePage)

function HomePage() {
    const navigate = useNavigate();
    const [selectedCategoryId, setSelectedCategoryId] = useState(null);
    const [selectedGender, setSelectedGender] = useState(null);
    const [activeFilter, setActiveFilter] = useState('Tất cả');

    const filterTabs = ['Tất cả', 'Thời trang', 'Nước hoa', 'Mỹ phẩm', 'Phụ kiện'];
    const categoryIcons = [
        { icon: '👔', label: 'Nam', gender: 'nam' },
        { icon: '👗', label: 'Nữ', gender: 'nu' },
        { icon: '🧒', label: 'Trẻ Em', gender: 'treem' },
        { icon: '🌸', label: 'Nước Hoa', gender: null },
        { icon: '💄', label: 'Mỹ Phẩm', gender: null },
        { icon: '👜', label: 'Phụ Kiện', gender: null },
    ];

    /* Click icon gender → vào trang /thoi-trang?gender=nam */
    const handleGenderClick = (gender) => {
        if (gender) navigate(`/thoi-trang?gender=${gender}`);
    };

    return (
        <>
            {/* ══ HERO BANNER ══ */}
            <section style={{ backgroundColor: C.cream, display: 'grid', gridTemplateColumns: '1fr 340px', maxWidth: '100%', minHeight: '320px' }}>
                <div style={{ padding: '56px 64px', display: 'flex', flexDirection: 'column', justifyContent: 'center' }}>
                    <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.72rem', letterSpacing: '3px', color: C.gold, textTransform: 'uppercase', marginBottom: '16px', fontWeight: 500 }}>BỘ SƯU TẬP 2026</span>
                    <h1 style={{ fontSize: '3rem', fontWeight: 300, lineHeight: 1.2, color: C.dark, margin: '0 0 8px 0' }}>Phong cách</h1>
                    <h1 style={{ fontSize: '3rem', fontWeight: 400, fontStyle: 'italic', lineHeight: 1.2, color: C.gold, margin: '0 0 20px 0' }}>không giới hạn tuổi tác</h1>
                    <p style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.9rem', color: C.muted, lineHeight: 1.7, maxWidth: '420px', margin: '0 0 32px 0', fontWeight: 300 }}>
                        Thời trang, nước hoa & mỹ phẩm cao cấp — dành cho mọi lứa tuổi, mọi phong cách.
                    </p>
                    <div style={{ display: 'flex', gap: '16px' }}>
                        <a href="#products-area"
                            style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.78rem', letterSpacing: '2px', fontWeight: 500, textTransform: 'uppercase', padding: '14px 32px', border: `1px solid ${C.dark}`, color: C.dark, textDecoration: 'none', backgroundColor: 'transparent', transition: 'all 0.25s' }}
                            onMouseEnter={e => { e.target.style.backgroundColor = C.dark; e.target.style.color = '#fff'; }}
                            onMouseLeave={e => { e.target.style.backgroundColor = 'transparent'; e.target.style.color = C.dark; }}>
                            KHÁM PHÁ NGAY
                        </a>
                        <Link to="/thoi-trang"
                            style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.78rem', letterSpacing: '2px', fontWeight: 500, textTransform: 'uppercase', padding: '14px 32px', border: `1px solid ${C.border}`, color: C.muted, textDecoration: 'none', backgroundColor: 'transparent', transition: 'all 0.25s' }}
                            onMouseEnter={e => { e.target.style.borderColor = C.gold; e.target.style.color = C.gold; }}
                            onMouseLeave={e => { e.target.style.borderColor = C.border; e.target.style.color = C.muted; }}>
                            XEM LOOKBOOK
                        </Link>
                    </div>
                </div>

                <div style={{ display: 'flex', flexDirection: 'column', borderLeft: `1px solid ${C.border}` }}>
                    {[
                        { icon: '👗', title: 'THỜI TRANG & PHỤ KIỆN', sub: 'Hơn 500 sản phẩm', bg: C.creamDark, to: '/thoi-trang' },
                        { icon: '🌸', title: 'NƯỚC HOA & MỸ PHẨM', sub: 'Hơn 200 sản phẩm', bg: '#f0eae2', to: '/shop' },
                    ].map((box, i) => (
                        <div key={i} style={{ flex: 1, padding: '32px', borderBottom: i === 0 ? `1px solid ${C.border}` : 'none', backgroundColor: box.bg, display: 'flex', flexDirection: 'column', justifyContent: 'center' }}>
                            <div style={{ fontSize: '1.5rem', marginBottom: '12px' }}>{box.icon}</div>
                            <p style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.7rem', letterSpacing: '2px', color: C.muted, textTransform: 'uppercase', margin: '0 0 4px 0' }}>{box.title}</p>
                            <p style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.78rem', color: C.muted, margin: '0 0 12px 0', fontWeight: 300 }}>{box.sub}</p>
                            <Link to={box.to} style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.72rem', letterSpacing: '1.5px', color: C.gold, textDecoration: 'none', textTransform: 'uppercase', fontWeight: 500 }}>MUA NGAY →</Link>
                        </div>
                    ))}
                </div>
            </section>

            {/* ══ CATEGORY ICON TABS ══ */}
            <section style={{ backgroundColor: C.white, borderTop: `1px solid ${C.border}`, borderBottom: `1px solid ${C.border}` }}>
                <div style={{ maxWidth: '1200px', margin: '0 auto', display: 'flex', justifyContent: 'center' }}>
                    {categoryIcons.map((cat, i) => (
                        <button key={i}
                            onClick={() => handleGenderClick(cat.gender)}
                            style={{
                                flex: 1, padding: '22px 16px', border: 'none',
                                borderRight: i < categoryIcons.length - 1 ? `1px solid ${C.border}` : 'none',
                                backgroundColor: 'transparent',
                                cursor: cat.gender ? 'pointer' : 'default',
                                display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '7px',
                                transition: 'background 0.2s',
                            }}
                            onMouseEnter={e => { if (cat.gender) e.currentTarget.style.backgroundColor = C.cream; }}
                            onMouseLeave={e => e.currentTarget.style.backgroundColor = 'transparent'}
                        >
                            <span style={{ fontSize: '1.3rem' }}>{cat.icon}</span>
                            <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.7rem', letterSpacing: '1.5px', color: C.dark, textTransform: 'uppercase', fontWeight: 500 }}>{cat.label}</span>
                        </button>
                    ))}
                </div>
            </section>

            {/* ══ SẢN PHẨM NỔI BẬT ══ */}
            <section id="products-area" style={{ maxWidth: '1200px', margin: '0 auto', padding: '56px 24px' }}>
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: '32px', flexWrap: 'wrap', gap: '16px' }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '14px' }}>
                        <h2 style={{ fontSize: '1.5rem', fontWeight: 400, color: C.dark, margin: 0 }}>Sản Phẩm Nổi Bật</h2>
                        {selectedGender && (
                            <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.7rem', letterSpacing: '1px', textTransform: 'uppercase', backgroundColor: C.gold, color: '#fff', padding: '4px 12px', display: 'flex', alignItems: 'center', gap: '6px' }}>
                                {GENDER_LABELS[selectedGender]}
                                <button onClick={() => { setSelectedGender(null); setSelectedCategoryId(null); }}
                                    style={{ background: 'none', border: 'none', color: '#fff', cursor: 'pointer', padding: 0, fontSize: '0.8rem', lineHeight: 1 }}>✕</button>
                            </span>
                        )}
                    </div>
                    <div style={{ display: 'flex', gap: '4px' }}>
                        {filterTabs.map(tab => (
                            <button key={tab} onClick={() => setActiveFilter(tab)}
                                style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.75rem', letterSpacing: '1px', padding: '7px 16px', border: `1px solid ${activeFilter === tab ? C.gold : C.border}`, backgroundColor: activeFilter === tab ? C.gold : 'transparent', color: activeFilter === tab ? '#fff' : C.muted, cursor: 'pointer', borderRadius: '2px', transition: 'all 0.2s', fontWeight: 500 }}>
                                {tab}
                            </button>
                        ))}
                    </div>
                </div>

                <div style={{ display: 'grid', gridTemplateColumns: '220px 1fr', gap: '40px' }}>
                    <aside>
                        <CategoryProductList selectedId={selectedCategoryId} onSelectCategory={setSelectedCategoryId} gender={selectedGender} categoryType="thoi-trang"   />
                        <div style={{ marginTop: '24px' }}><BlogCategoryList /></div>
                    </aside>
                    <div>
                        {(selectedCategoryId || selectedGender) && (
                            <div style={{ marginBottom: '16px' }}>
                                <button onClick={() => { setSelectedCategoryId(null); setSelectedGender(null); }}
                                    style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.75rem', letterSpacing: '1px', padding: '6px 14px', border: `1px solid ${C.border}`, backgroundColor: 'transparent', color: C.muted, cursor: 'pointer', borderRadius: '2px' }}>
                                    ✕ Xem tất cả
                                </button>
                            </div>
                        )}
                        <ProductList categoryId={selectedCategoryId} gender={selectedGender} />
                    </div>
                </div>
            </section>
        </>
    );
}

  // FOOTER

function Footer() {
    return (
        <footer style={{ backgroundColor: C.dark, borderTop: '1px solid #2a2a2a', marginTop: '48px' }}>
            <div style={{ maxWidth: '1200px', margin: '0 auto', padding: '20px 24px', display: 'flex', alignItems: 'center', justifyContent: 'space-between', gap: '24px', flexWrap: 'wrap' }}>
                <div style={{ display: 'flex', alignItems: 'baseline', gap: '8px' }}>
                    <span style={{ fontSize: '0.9rem', fontWeight: 600, letterSpacing: '3px', color: '#fff', textTransform: 'uppercase', fontFamily: "'Cormorant Garamond',Georgia,serif" }}>MAI TRINH</span>
                    <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.6rem', letterSpacing: '4px', color: C.gold, textTransform: 'uppercase' }}>STUDIO</span>
                </div>
                <div style={{ display: 'flex', gap: '24px', alignItems: 'center' }}>
                    {[{ icon: '📍', text: 'TP. Hồ Chí Minh' }, { icon: '📞', text: '0825644042' }, { icon: '✉️', text: 'hambr2802@gmail.com' }].map(item => (
                        <span key={item.text} style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.75rem', color: '#888', fontWeight: 300, display: 'flex', alignItems: 'center', gap: '5px' }}>
                            <span style={{ fontSize: '0.7rem' }}>{item.icon}</span>{item.text}
                        </span>
                    ))}
                </div>
                <div style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.72rem', color: '#666', fontWeight: 300, textAlign: 'right', lineHeight: 1.6 }}>
                    Nguyễn Văn Hà · 2122110132 · CCQ2211D
                </div>
            </div>
            <div style={{ borderTop: '1px solid #222', padding: '10px 24px', textAlign: 'center' }}>
                <p style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.68rem', color: '#444', margin: 0, letterSpacing: '1px' }}>© 2026 MAI TRINH STUDIO — All rights reserved</p>
            </div>
        </footer>
    );
}


  // APP ROOT — định nghĩa Routes

function App() {
    return (
        <div style={{ fontFamily: "'Cormorant Garamond','Playfair Display',Georgia,serif", backgroundColor: C.cream, minHeight: '100vh' }}>
            <link href="https://fonts.googleapis.com/css2?family=Cormorant+Garamond:ital,wght@0,300;0,400;0,600;1,400&family=Jost:wght@300;400;500&display=swap" rel="stylesheet" />

            <Header />

            <Routes>
                <Route path="/" element={<HomePage />} />
                <Route path="/thoi-trang" element={<Shop />} />
                <Route path="/thoi-trang/:gender" element={<FashionPage />} />
                <Route path="/nuoc-hoa" element={<Shop type="nuoc-hoa" />} />
                <Route path="/my-pham" element={<Shop type="my-pham" />} />
                <Route path="/phu-kien" element={<Shop type="phu-kien" />} />
                <Route path="/login" element={<LoginPage />} />
                <Route path="/register" element={<RegisterPage />} />
                <Route path="/profile" element={<ProfilePage />} />
                <Route path="/cart" element={<CartPage />} />
                <Route path="/checkout" element={<CheckoutPage />} />
            </Routes>

            <Footer />
        </div>
    );
}

export default App;