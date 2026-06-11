/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : FashionPage.jsx - Trang /thoi-trang chuyên nghiệp
               3 tab Nam / Nữ / Trẻ Em + đề xuất bộ đồ + thanh tìm kiếm
               Gọi API qua productService & categoryProductService
*/
import React, { useState, useEffect, useRef } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import productService from '../api/productService';
import categoryProductService from '../services/categoryProductService';

/* ─── Design tokens (đồng bộ với App.js) ─── */
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

/* ─── Cấu hình 3 giới tính ─── */
const GENDERS = [
    {
        key: 'nam',
        label: 'Thời Trang Nam',
        short: 'Nam',
        icon: '👔',
        tagline: 'Lịch lãm & hiện đại',
        hero: 'https://images.unsplash.com/photo-1617137968427-85924c800a22?q=80&w=800',
        accent: '#1a1a1a',
    },
    {
        key: 'nu',
        label: 'Thời Trang Nữ',
        short: 'Nữ',
        icon: '👗',
        tagline: 'Tinh tế & quyến rũ',
        hero: 'https://images.unsplash.com/photo-1469334031218-e382a71b716b?q=80&w=800',
        accent: '#b8975a',
    },
    {
        key: 'treem',
        label: 'Thời Trang Trẻ Em',
        short: 'Trẻ Em',
        icon: '🧒',
        tagline: 'Năng động & thoải mái',
        hero: 'https://images.unsplash.com/photo-1503944583220-79d8926ad5e2?q=80&w=800',
        accent: '#5a8ab8',
    },
];

/* ─── Đề xuất bộ đồ mỗi giới tính ─── */
const OUTFIT_SUGGESTIONS = {
    nam: [
        {
            id: 1,
            title: 'Business Casual',
            desc: 'Áo sơ mi trắng + quần tây xám + giày Oxford',
            tags: ['Công sở', 'Lịch sự'],
            img: 'https://images.unsplash.com/photo-1594938298603-c8148c4b4571?q=80&w=400',
            badge: 'Phổ biến',
        },
        {
            id: 2,
            title: 'Smart Casual',
            desc: 'Áo polo + quần chinos + giày sneaker trắng',
            tags: ['Cuối tuần', 'Thoải mái'],
            img: 'https://images.unsplash.com/photo-1552374196-1ab2a1c593e8?q=80&w=400',
            badge: 'Xu hướng',
        },
        {
            id: 3,
            title: 'Street Style',
            desc: 'Áo hoodie + quần jogger + sneaker cao cổ',
            tags: ['Dạo phố', 'Năng động'],
            img: 'https://images.unsplash.com/photo-1583743814966-8936f5b7be1a?q=80&w=400',
            badge: 'Hot',
        },
        {
            id: 4,
            title: 'Formal Suit',
            desc: 'Bộ vest đen + áo sơ mi trắng + cà vạt',
            tags: ['Sự kiện', 'Sang trọng'],
            img: 'https://images.unsplash.com/photo-1507679799987-c73779587ccf?q=80&w=400',
            badge: 'Mới',
        },
    ],
    nu: [
        {
            id: 1,
            title: 'Office Chic',
            desc: 'Đầm công sở + blazer + giày cao gót',
            tags: ['Công sở', 'Thanh lịch'],
            img: 'https://images.unsplash.com/photo-1539109136881-3be0616acf4b?q=80&w=400',
            badge: 'Phổ biến',
        },
        {
            id: 2,
            title: 'Casual Boho',
            desc: 'Áo croptop + chân váy dài + sandal',
            tags: ['Dạo phố', 'Bohemian'],
            img: 'https://images.unsplash.com/photo-1515886657613-9f3515b0c78f?q=80&w=400',
            badge: 'Xu hướng',
        },
        {
            id: 3,
            title: 'Date Night',
            desc: 'Đầm dự tiệc + giày cao gót + clutch',
            tags: ['Hẹn hò', 'Quyến rũ'],
            img: 'https://images.unsplash.com/photo-1496747611176-843222e1e57c?q=80&w=400',
            badge: 'Hot',
        },
        {
            id: 4,
            title: 'Active & Fresh',
            desc: 'Áo thể thao + quần legging + sneaker',
            tags: ['Thể thao', 'Năng động'],
            img: 'https://images.unsplash.com/photo-1506629082955-511b1aa562c8?q=80&w=400',
            badge: 'Mới',
        },
    ],
    treem: [
        {
            id: 1,
            title: 'Đi Học Năng Động',
            desc: 'Áo thun + quần jeans + giày thể thao',
            tags: ['Đi học', 'Thoải mái'],
            img: 'https://images.unsplash.com/photo-1519238263530-99bdd11df2ea?q=80&w=400',
            badge: 'Phổ biến',
        },
        {
            id: 2,
            title: 'Bé Gái Dễ Thương',
            desc: 'Đầm công chúa + giày búp bê',
            tags: ['Dự tiệc', 'Đáng yêu'],
            img: 'https://images.unsplash.com/photo-1503944583220-79d8926ad5e2?q=80&w=400',
            badge: 'Yêu thích',
        },
        {
            id: 3,
            title: 'Bộ Thể Thao Bé Trai',
            desc: 'Áo thun thể thao + quần shorts + sneaker',
            tags: ['Vận động', 'Năng động'],
            img: 'https://images.unsplash.com/photo-1484863137850-59afcfe05386?q=80&w=400',
            badge: 'Hot',
        },
        {
            id: 4,
            title: 'Dạo Phố Cuối Tuần',
            desc: 'Áo hoodie + jeans + giày thể thao màu sắc',
            tags: ['Cuối tuần', 'Vui vẻ'],
            img: 'https://images.unsplash.com/photo-1471286174890-9c112ffca5b4?q=80&w=400',
            badge: 'Mới',
        },
    ],
};

const BADGE_COLORS = {
    'Phổ biến': C.dark,
    'Xu hướng': '#7b5ea7',
    'Hot': '#c0392b',
    'Mới': C.gold,
    'Yêu thích': '#e05c7e',
};

/* ══════════════════════════════════════════
   PRODUCT CARD
══════════════════════════════════════════ */
function ProductCard({ item, onClick }) {
    const [hovered, setHovered] = useState(false);
    return (
        <div
            onClick={onClick}
            onMouseEnter={() => setHovered(true)}
            onMouseLeave={() => setHovered(false)}
            style={{
                backgroundColor: hovered ? C.cream : C.white,
                border: `1px solid ${hovered ? C.gold : C.border}`,
                cursor: 'pointer',
                transition: 'all 0.2s',
                display: 'flex',
                flexDirection: 'column',
                position: 'relative',
                overflow: 'hidden',
            }}
        >
            {/* Badge */}
            <div style={{ position: 'absolute', top: '10px', left: '10px', zIndex: 2, display: 'flex', gap: '4px' }}>
                {item.isNew && <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.58rem', letterSpacing: '1.5px', textTransform: 'uppercase', backgroundColor: C.dark, color: '#fff', padding: '3px 7px' }}>Mới</span>}
                {item.isSale && <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.58rem', letterSpacing: '1.5px', textTransform: 'uppercase', backgroundColor: '#c0392b', color: '#fff', padding: '3px 7px' }}>Sale</span>}
                {item.isHot && <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.58rem', letterSpacing: '1.5px', textTransform: 'uppercase', backgroundColor: C.gold, color: '#fff', padding: '3px 7px' }}>Hot</span>}
            </div>

            {/* Ảnh */}
            <div style={{ height: '200px', overflow: 'hidden', backgroundColor: C.cream }}>
                <img
                    src={item.imageUrl || 'https://images.unsplash.com/photo-1523381210434-271e8be1f52b?q=80&w=400'}
                    alt={item.name}
                    style={{ width: '100%', height: '100%', objectFit: 'cover', transform: hovered ? 'scale(1.05)' : 'scale(1)', transition: 'transform 0.4s' }}
                />
            </div>

            {/* Info */}
            <div style={{ padding: '16px' }}>
                <p style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.68rem', color: C.gold, letterSpacing: '1px', textTransform: 'uppercase', margin: '0 0 4px', fontWeight: 500 }}>
                    {item.categoryName || ''}
                </p>
                <h4 style={{ fontSize: '0.92rem', fontWeight: 400, color: C.dark, margin: '0 0 10px', lineHeight: 1.4 }}>
                    {item.name}
                </h4>
                {item.sizes && item.sizes.length > 0 && (
                    <div style={{ display: 'flex', gap: '4px', marginBottom: '10px', flexWrap: 'wrap' }}>
                        {item.sizes.map(s => (
                            <span key={s} style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.6rem', letterSpacing: '1px', padding: '2px 6px', border: `1px solid ${C.border}`, color: C.muted }}>
                                {s}
                            </span>
                        ))}
                    </div>
                )}
                <div style={{ display: 'flex', alignItems: 'baseline', gap: '8px' }}>
                    {item.originalPrice && item.originalPrice > item.price && (
                        <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.72rem', color: C.muted, textDecoration: 'line-through', fontWeight: 300 }}>
                            {item.originalPrice.toLocaleString('vi-VN')}₫
                        </span>
                    )}
                    <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.9rem', fontWeight: 500, color: C.dark }}>
                        {item.price ? item.price.toLocaleString('vi-VN') + ' ₫' : 'Liên hệ'}
                    </span>
                </div>
            </div>
        </div>
    );
}

/* ══════════════════════════════════════════
   OUTFIT CARD
══════════════════════════════════════════ */
function OutfitCard({ outfit }) {
    const [hovered, setHovered] = useState(false);
    return (
        <div
            onMouseEnter={() => setHovered(true)}
            onMouseLeave={() => setHovered(false)}
            style={{
                backgroundColor: C.white,
                border: `1px solid ${hovered ? C.gold : C.border}`,
                overflow: 'hidden',
                cursor: 'pointer',
                transition: 'all 0.2s',
                boxShadow: hovered ? '0 8px 24px rgba(184,151,90,0.12)' : 'none',
            }}
        >
            {/* Ảnh */}
            <div style={{ height: '220px', overflow: 'hidden', position: 'relative' }}>
                <img
                    src={outfit.img}
                    alt={outfit.title}
                    style={{ width: '100%', height: '100%', objectFit: 'cover', transform: hovered ? 'scale(1.06)' : 'scale(1)', transition: 'transform 0.4s' }}
                />
                {/* Badge */}
                <div style={{
                    position: 'absolute', top: '12px', left: '12px',
                    backgroundColor: BADGE_COLORS[outfit.badge] || C.dark,
                    color: '#fff',
                    fontFamily: "'Jost',sans-serif",
                    fontSize: '0.6rem',
                    letterSpacing: '1.5px',
                    textTransform: 'uppercase',
                    padding: '4px 10px',
                }}>
                    {outfit.badge}
                </div>
            </div>

            {/* Info */}
            <div style={{ padding: '18px 20px' }}>
                <h4 style={{ fontSize: '1rem', fontWeight: 400, color: C.dark, margin: '0 0 6px', letterSpacing: '0.5px' }}>
                    {outfit.title}
                </h4>
                <p style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.78rem', color: C.muted, margin: '0 0 12px', fontWeight: 300, lineHeight: 1.5 }}>
                    {outfit.desc}
                </p>
                <div style={{ display: 'flex', gap: '6px', flexWrap: 'wrap' }}>
                    {outfit.tags.map(tag => (
                        <span key={tag} style={{
                            fontFamily: "'Jost',sans-serif",
                            fontSize: '0.65rem',
                            letterSpacing: '1px',
                            padding: '3px 10px',
                            border: `1px solid ${C.border}`,
                            color: C.muted,
                            backgroundColor: C.cream,
                        }}>
                            {tag}
                        </span>
                    ))}
                </div>
            </div>
        </div>
    );
}

/* ══════════════════════════════════════════
   COMPONET CHỨA 3 Ô DANH MỤC LỚN HÀNG NGANG
══════════════════════════════════════════ */
function HorizontalCategorySelector({ activeGender, onSelect }) {
    return (
        <div style={{
            display: 'grid',
            gridTemplateColumns: 'repeat(3, 1fr)',
            gap: '24px',
            marginBottom: '40px'
        }}>
            {GENDERS.map((g) => {
                const isActive = activeGender === g.key;
                return (
                    <div
                        key={g.key}
                        onClick={() => onSelect(g.key)}
                        className={`custom-cat-card ${isActive ? 'active' : ''}`}
                        style={{
                            backgroundColor: isActive ? '#fcfbf7' : C.white,
                            border: `1px solid ${isActive ? C.gold : C.border}`,
                            padding: '30px 20px',
                            textAlign: 'center',
                            cursor: 'pointer',
                            transition: 'all 0.3s ease',
                        }}
                    >
                        <div style={{ fontSize: '2.5rem', marginBottom: '12px' }}>{g.icon}</div>
                        <h3 style={{
                            fontFamily: "'Jost', sans-serif",
                            fontSize: '1.05rem',
                            fontWeight: isActive ? 600 : 400,
                            letterSpacing: '2px',
                            textTransform: 'uppercase',
                            margin: '0 0 6px',
                            color: isActive ? C.gold : C.dark
                        }}>
                            {g.short}
                        </h3>
                        <p style={{
                            fontFamily: "'Jost', sans-serif",
                            fontSize: '0.78rem',
                            color: C.muted,
                            margin: 0,
                            fontWeight: 300
                        }}>
                            {g.tagline}
                        </p>
                    </div>
                );
            })}
        </div>
    );
}

/* ══════════════════════════════════════════
   MAIN FASHION PAGE
══════════════════════════════════════════ */
const FashionPage = () => {
    const [searchParams, setSearchParams] = useSearchParams();
    const initGender = searchParams.get('gender') || null;
    const [activeGender, setActiveGender] = useState(initGender);
    const [searchQuery, setSearchQuery] = useState('');
    const [categories, setCategories] = useState([]);
    const [selectedCategory, setSelectedCategory] = useState(null);
    const [products, setProducts] = useState([]);
    const [productsLoading, setProductsLoading] = useState(false);
    const [productsError, setProductsError] = useState(null);
    const [selectedSize, setSelectedSize] = useState(null);
    const [showProducts, setShowProducts] = useState(false);
    const productSectionRef = useRef(null);
    const navigate = useNavigate();
    const SIZES = ['S', 'M', 'L', 'XL', 'XXL'];
    useEffect(() => {
        const genderFromUrl = searchParams.get('gender');
        if (genderFromUrl && genderFromUrl !== activeGender) {
            setActiveGender(genderFromUrl);
        }
    }, [searchParams]);
    const activeGenderInfo = GENDERS.find(g => g.key === activeGender);
    const outfits = OUTFIT_SUGGESTIONS[activeGender] || [];

    /* Cập nhật URL khi đổi gender */
    useEffect(() => {
        setSearchParams({ gender: activeGender });
        setSearchQuery('');
        setSelectedCategory(null);
        setProducts([]);
        setSelectedSize(null);
        setShowProducts(false);
    }, [activeGender]);

    /* Load categories theo gender */
    useEffect(() => {
        const fetchCategories = async () => {
            try {
                const res = await productService.getCategories(activeGender);
                setCategories(Array.isArray(res) ? res : (res.data || []));
            } catch (err) {
                console.error('Lỗi tải danh mục:', err);
                setCategories([]);
            }
        };
        fetchCategories();
    }, [activeGender]);

    /* Load products khi chọn category */
    useEffect(() => {
        if (selectedCategory === null) return;
        const fetchProducts = async () => {
            try {
                setProductsLoading(true);
                setProductsError(null);
                const res = await productService.getProductsByCategory(selectedCategory);
                setProducts(res.data || res);
            } catch (err) {
                setProductsError('Không thể tải sản phẩm. Vui lòng thử lại!');
            } finally {
                setProductsLoading(false);
            }
        };
        fetchProducts();
    }, [selectedCategory]);

    /* Lọc sản phẩm theo search + size */
    const filteredProducts = products
        .filter(p => !searchQuery || p.name?.toLowerCase().includes(searchQuery.toLowerCase()))
        .filter(p => !selectedSize || (p.sizes && p.sizes.includes(selectedSize)));

    const handleSelectGender = (key) => {
        setActiveGender(key);
        window.scrollTo({ top: 0, behavior: 'smooth' });
    };

    const handleExploreProducts = async () => {
        setShowProducts(true);
        if (selectedCategory === null) {
            try {
                setProductsLoading(true);
                setProductsError(null);
                const res = await productService.getProductsByGender(activeGender);
                setProducts(res.data || res);
            } catch (err) {
                setProductsError('Không thể tải sản phẩm. Vui lòng thử lại!');
            } finally {
                setProductsLoading(false);
            }
        }
        setTimeout(() => {
            productSectionRef.current?.scrollIntoView({ behavior: 'smooth', block: 'start' });
        }, 100);
    };

    return (
        <div style={{
            fontFamily: "'Cormorant Garamond','Playfair Display',Georgia,serif",
            backgroundColor: C.cream,
            minHeight: '100vh',
        }}>
            <link href="https://fonts.googleapis.com/css2?family=Cormorant+Garamond:ital,wght@0,300;0,400;0,600;1,400&family=Jost:wght@300;400;500&display=swap" rel="stylesheet" />

            {/* ══ BREADCRUMB ══ */}
            <div style={{ backgroundColor: C.white, borderBottom: `1px solid ${C.border}`, padding: '10px 40px' }}>
                <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.72rem', color: C.muted, letterSpacing: '0.5px' }}>
                    <span
                        onClick={() => navigate('/')}
                        style={{ cursor: 'pointer', color: C.muted }}
                        onMouseEnter={e => e.target.style.color = C.gold}
                        onMouseLeave={e => e.target.style.color = C.muted}
                    >
                        Trang Chủ
                    </span>
                    <span style={{ margin: '0 8px', opacity: 0.4 }}>›</span>
                    <span style={{ color: C.dark, fontWeight: 500 }}>Thời Trang</span>
                    {activeGenderInfo && (
                        <>
                            <span style={{ margin: '0 8px', opacity: 0.4 }}>›</span>
                            <span style={{ color: C.gold }}>{activeGenderInfo.short}</span>
                        </>
                    )}
                </span>
            </div>

           

            {/* ══ HERO SECTION ══ */}
            <section style={{
                position: 'relative',
                height: '420px',
                overflow: 'hidden',
            }}>
                <img
                    key={activeGender}
                    src={activeGenderInfo?.hero}
                    alt={activeGenderInfo?.label}
                    style={{
                        width: '100%',
                        height: '100%',
                        objectFit: 'cover',
                        objectPosition: 'center 20%',
                        filter: 'brightness(0.55)',
                        animation: 'fadeIn 0.6s ease',
                    }}
                />
                <div style={{
                    position: 'absolute', inset: 0,
                    background: 'linear-gradient(to right, rgba(0,0,0,0.6) 0%, rgba(0,0,0,0.1) 60%, transparent 100%)',
                }} />

                <div style={{
                    position: 'absolute',
                    top: '50%',
                    left: '64px',
                    transform: 'translateY(-50%)',
                }}>
                    <span style={{
                        fontFamily: "'Jost',sans-serif",
                        fontSize: '0.72rem',
                        letterSpacing: '4px',
                        color: C.gold,
                        textTransform: 'uppercase',
                        display: 'block',
                        marginBottom: '12px',
                        fontWeight: 500,
                    }}>
                        BỘ SƯU TẬP 2026
                    </span>
                    <h1 style={{
                        fontSize: '3.2rem',
                        fontWeight: 300,
                        color: '#fff',
                        margin: '0 0 8px',
                        lineHeight: 1.15,
                        letterSpacing: '1px',
                    }}>
                        {activeGenderInfo?.label}
                    </h1>
                    <p style={{
                        fontFamily: "'Jost',sans-serif",
                        fontSize: '0.95rem',
                        color: 'rgba(255,255,255,0.75)',
                        margin: '0 0 32px',
                        fontWeight: 300,
                        letterSpacing: '0.5px',
                    }}>
                        {activeGenderInfo?.tagline} — Khám phá những xu hướng mới nhất
                    </p>
                    <div style={{ display: 'flex', gap: '12px' }}>
                        <button
                            onClick={handleExploreProducts}
                            style={{
                                fontFamily: "'Jost',sans-serif",
                                fontSize: '0.78rem',
                                letterSpacing: '2px',
                                fontWeight: 500,
                                textTransform: 'uppercase',
                                padding: '13px 28px',
                                backgroundColor: C.gold,
                                color: '#fff',
                                border: 'none',
                                cursor: 'pointer',
                                transition: 'all 0.2s',
                            }}
                            onMouseEnter={e => e.currentTarget.style.backgroundColor = '#a07840'}
                            onMouseLeave={e => e.currentTarget.style.backgroundColor = C.gold}
                        >
                            Xem Sản Phẩm →
                        </button>
                        <button
                            onClick={() => document.getElementById('outfit-section')?.scrollIntoView({ behavior: 'smooth' })}
                            style={{
                                fontFamily: "'Jost',sans-serif",
                                fontSize: '0.78rem',
                                letterSpacing: '2px',
                                fontWeight: 500,
                                textTransform: 'uppercase',
                                padding: '13px 28px',
                                backgroundColor: 'transparent',
                                color: '#fff',
                                border: '1px solid rgba(255,255,255,0.5)',
                                cursor: 'pointer',
                                transition: 'all 0.2s',
                            }}
                            onMouseEnter={e => { e.currentTarget.style.borderColor = '#fff'; e.currentTarget.style.backgroundColor = 'rgba(255,255,255,0.1)'; }}
                            onMouseLeave={e => { e.currentTarget.style.borderColor = 'rgba(255,255,255,0.5)'; e.currentTarget.style.backgroundColor = 'transparent'; }}
                        >
                            Gợi Ý Phối Đồ
                        </button>
                    </div>
                </div>
            </section>

            {/* ══ OUTFIT SUGGESTIONS ══ */}
            <section id="outfit-section" style={{ maxWidth: '1200px', margin: '0 auto', padding: '60px 24px' }}>
                <div style={{ display: 'flex', alignItems: 'flex-end', justifyContent: 'space-between', marginBottom: '36px' }}>
                    <div>
                        <p style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.68rem', letterSpacing: '3px', color: C.gold, textTransform: 'uppercase', margin: '0 0 8px', fontWeight: 500 }}>
                            GỢI Ý PHỐI ĐỒ
                        </p>
                        <h2 style={{ fontSize: '1.8rem', fontWeight: 300, color: C.dark, margin: 0 }}>
                            Bộ Đồ <em style={{ fontStyle: 'italic', color: C.gold }}>Đề Xuất</em> cho {activeGenderInfo?.short}
                        </h2>
                    </div>
                    <button
                        onClick={handleExploreProducts}
                        style={{
                            fontFamily: "'Jost',sans-serif",
                            fontSize: '0.72rem',
                            letterSpacing: '1.5px',
                            textTransform: 'uppercase',
                            color: C.gold,
                            background: 'none',
                            border: `1px solid ${C.gold}`,
                            cursor: 'pointer',
                            padding: '10px 20px',
                            transition: 'all 0.2s',
                            fontWeight: 500,
                        }}
                        onMouseEnter={e => { e.currentTarget.style.backgroundColor = C.gold; e.currentTarget.style.color = '#fff'; }}
                        onMouseLeave={e => { e.currentTarget.style.backgroundColor = 'transparent'; e.currentTarget.style.color = C.gold; }}
                    >
                        Mua ngay →
                    </button>
                </div>

                <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '20px' }}>
                    {outfits.map(outfit => <OutfitCard key={outfit.id} outfit={outfit} />)}
                </div>
            </section>

            {/* ══ DIVIDER ══ */}
            <div style={{ borderTop: `1px solid ${C.border}`, maxWidth: '1200px', margin: '0 auto' }} />

            {/* ══ PRODUCT SECTION ══ */}
            <section ref={productSectionRef} style={{ maxWidth: '1200px', margin: '0 auto', padding: '60px 24px' }}>

                {/* ── CHÈN 3 Ô DANH MỤC HÀNG NGANG VÀO ĐÂY ── */}
                <HorizontalCategorySelector
                    activeGender={activeGender}
                    onSelect={handleSelectGender}
                />

                {/* Tiêu đề */}
                <div style={{ marginBottom: '32px' }}>
                    <p style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.68rem', letterSpacing: '3px', color: C.gold, textTransform: 'uppercase', margin: '0 0 8px', fontWeight: 500 }}>
                        SẢN PHẨM
                    </p>
                    <h2 style={{ fontSize: '1.8rem', fontWeight: 300, color: C.dark, margin: 0 }}>
                        Khám Phá <em style={{ fontStyle: 'italic', color: C.gold }}>Tất Cả Sản Phẩm</em>
                    </h2>
                </div>

                <div style={{ display: 'grid', gridTemplateColumns: '240px 1fr', gap: '32px', alignItems: 'start' }}>

                    {/* ── SIDEBAR ── */}
                    <aside>
                        {/* Thanh tìm kiếm */}
                        <div style={{ marginBottom: '20px' }}>
                            <p style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.65rem', letterSpacing: '2.5px', color: C.muted, textTransform: 'uppercase', margin: '0 0 10px', fontWeight: 500 }}>
                                TÌM KIẾM
                            </p>
                            <div style={{ position: 'relative' }}>
                                <input
                                    type="text"
                                    placeholder="Tìm sản phẩm..."
                                    value={searchQuery}
                                    onChange={e => setSearchQuery(e.target.value)}
                                    style={{
                                        width: '100%',
                                        padding: '10px 36px 10px 14px',
                                        fontFamily: "'Jost',sans-serif",
                                        fontSize: '0.78rem',
                                        color: C.dark,
                                        backgroundColor: C.white,
                                        border: `1px solid ${C.border}`,
                                        outline: 'none',
                                        boxSizing: 'border-box',
                                        letterSpacing: '0.5px',
                                        transition: 'border-color 0.2s',
                                    }}
                                    onFocus={e => e.target.style.borderColor = C.gold}
                                    onBlur={e => e.target.style.borderColor = C.border}
                                />
                                <span style={{
                                    position: 'absolute', right: '12px', top: '50%', transform: 'translateY(-50%)',
                                    color: C.muted, fontSize: '0.8rem', pointerEvents: 'none',
                                }}>
                                    🔍
                                </span>
                            </div>
                            {searchQuery && (
                                <button
                                    onClick={() => setSearchQuery('')}
                                    style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.68rem', color: C.gold, background: 'none', border: 'none', cursor: 'pointer', padding: '4px 0', letterSpacing: '0.5px' }}>
                                    ✕ Xoá tìm kiếm
                                </button>
                            )}
                        </div>

                        {/* Lọc theo size */}
                        {selectedCategory !== null && (
                            <div style={{ marginBottom: '20px', backgroundColor: C.white, border: `1px solid ${C.border}`, padding: '16px 18px' }}>
                                <p style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.65rem', letterSpacing: '2.5px', color: C.muted, textTransform: 'uppercase', margin: '0 0 12px', fontWeight: 500 }}>
                                    SIZE
                                </p>
                                <div style={{ display: 'flex', flexWrap: 'wrap', gap: '6px' }}>
                                    {['ALL', ...SIZES].map(s => (
                                        <button
                                            key={s}
                                            onClick={() => setSelectedSize(s === 'ALL' ? null : s)}
                                            style={{
                                                fontFamily: "'Jost',sans-serif",
                                                fontSize: '0.7rem',
                                                letterSpacing: '1px',
                                                width: '40px',
                                                height: '36px',
                                                border: `1px solid ${(s === 'ALL' ? selectedSize === null : selectedSize === s) ? C.gold : C.border}`,
                                                backgroundColor: (s === 'ALL' ? selectedSize === null : selectedSize === s) ? C.gold : 'transparent',
                                                color: (s === 'ALL' ? selectedSize === null : selectedSize === s) ? '#fff' : C.muted,
                                                cursor: 'pointer',
                                                transition: 'all 0.15s',
                                                fontWeight: 500,
                                            }}
                                        >
                                            {s}
                                        </button>
                                    ))}
                                </div>
                            </div>
                        )}

                        {/* Danh mục */}
                        <div style={{ backgroundColor: C.white, border: `1px solid ${C.border}` }}>
                            <div style={{ padding: '16px 18px 12px', borderBottom: `1px solid ${C.border}` }}>
                                <p style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.65rem', letterSpacing: '2.5px', color: C.muted, textTransform: 'uppercase', margin: 0, fontWeight: 500 }}>
                                    DANH MỤC
                                </p>
                                <div style={{ marginTop: '8px', display: 'flex', alignItems: 'center', gap: '8px' }}>
                                    <span style={{ fontSize: '1rem' }}>{activeGenderInfo?.icon}</span>
                                    <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.72rem', letterSpacing: '1px', color: C.gold, fontWeight: 600 }}>
                                        {activeGenderInfo?.label}
                                    </span>
                                </div>
                            </div>

                            {/* Tất cả */}
                            <button
                                onClick={() => { setSelectedCategory(null); setProducts([]); setSelectedSize(null); setShowProducts(false); }}
                                style={{
                                    width: '100%',
                                    padding: '12px 18px',
                                    display: 'flex',
                                    justifyContent: 'space-between',
                                    alignItems: 'center',
                                    border: 'none',
                                    borderBottom: `1px solid ${C.border}`,
                                    backgroundColor: selectedCategory === null ? C.dark : 'transparent',
                                    cursor: 'pointer',
                                    transition: 'all 0.15s',
                                }}
                                onMouseEnter={e => { if (selectedCategory !== null) e.currentTarget.style.backgroundColor = C.cream; }}
                                onMouseLeave={e => { if (selectedCategory !== null) e.currentTarget.style.backgroundColor = 'transparent'; }}
                            >
                                <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.78rem', color: selectedCategory === null ? '#fff' : C.dark, fontWeight: selectedCategory === null ? 500 : 300 }}>
                                    Tất Cả
                                </span>
                                <span style={{ fontSize: '0.6rem', color: selectedCategory === null ? '#fff' : C.muted, opacity: 0.6 }}>›</span>
                            </button>

                            {/* Danh sách category */}
                            {categories.map(cat => (
                                <button
                                    key={cat.id}
                                    onClick={() => { setSelectedCategory(cat.id); setSelectedSize(null); setShowProducts(true); }}
                                    style={{
                                        width: '100%',
                                        padding: '12px 18px',
                                        display: 'flex',
                                        justifyContent: 'space-between',
                                        alignItems: 'center',
                                        border: 'none',
                                        borderBottom: `1px solid ${C.border}`,
                                        backgroundColor: selectedCategory === cat.id ? C.dark : 'transparent',
                                        cursor: 'pointer',
                                        transition: 'all 0.15s',
                                    }}
                                    onMouseEnter={e => { if (selectedCategory !== cat.id) e.currentTarget.style.backgroundColor = C.cream; }}
                                    onMouseLeave={e => { if (selectedCategory !== cat.id) e.currentTarget.style.backgroundColor = 'transparent'; }}
                                >
                                    <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.78rem', color: selectedCategory === cat.id ? '#fff' : C.dark, fontWeight: selectedCategory === cat.id ? 500 : 300, letterSpacing: '0.3px' }}>
                                        {cat.name}
                                    </span>
                                    <span style={{ fontSize: '0.6rem', color: selectedCategory === cat.id ? '#fff' : C.muted, opacity: 0.6 }}>›</span>
                                </button>
                            ))}
                        </div>
                    </aside>

                    {/* ── PRODUCT AREA ── */}
                    <div>
                        {/* Không có gì được chọn */}
                        {!showProducts && selectedCategory === null && (
                            <div style={{
                                backgroundColor: C.white,
                                border: `1px solid ${C.border}`,
                                padding: '80px 40px',
                                textAlign: 'center',
                            }}>
                                <div style={{ fontSize: '3rem', marginBottom: '20px', opacity: 0.25 }}>{activeGenderInfo?.icon}</div>
                                <p style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.85rem', letterSpacing: '2px', color: C.muted, textTransform: 'uppercase', margin: '0 0 20px' }}>
                                    Chọn danh mục để xem sản phẩm
                                </p>
                                <button
                                    onClick={() => { setSelectedCategory(null); setShowProducts(true); handleExploreProducts(); }}
                                    style={{
                                        fontFamily: "'Jost',sans-serif",
                                        fontSize: '0.72rem',
                                        letterSpacing: '1.5px',
                                        textTransform: 'uppercase',
                                        color: C.gold,
                                        background: 'none',
                                        border: `1px solid ${C.gold}`,
                                        cursor: 'pointer',
                                        padding: '10px 24px',
                                        fontWeight: 500,
                                    }}>
                                    Xem tất cả sản phẩm →
                                </button>
                            </div>
                        )}

                        {/* Header kết quả */}
                        {(showProducts || selectedCategory !== null) && (
                            <>
                                <div style={{
                                    display: 'flex',
                                    alignItems: 'center',
                                    justifyContent: 'space-between',
                                    marginBottom: '16px',
                                    padding: '14px 20px',
                                    backgroundColor: C.white,
                                    border: `1px solid ${C.border}`,
                                }}>
                                    <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                                        <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.78rem', color: C.dark, letterSpacing: '0.5px' }}>
                                            {productsLoading ? 'Đang tải...' : `${filteredProducts.length} sản phẩm`}
                                        </span>
                                        {searchQuery && (
                                            <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.72rem', color: C.gold, letterSpacing: '0.5px' }}>
                                                — kết quả cho "{searchQuery}"
                                            </span>
                                        )}
                                    </div>
                                    {selectedCategory !== null && (
                                        <button
                                            onClick={() => { setSelectedCategory(null); setProducts([]); setSelectedSize(null); setShowProducts(false); }}
                                            style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.7rem', letterSpacing: '1px', padding: '5px 12px', border: `1px solid ${C.border}`, backgroundColor: 'transparent', color: C.muted, cursor: 'pointer' }}>
                                            ✕ Xem tất cả
                                        </button>
                                    )}
                                </div>

                                {/* Loading */}
                                {productsLoading && (
                                    <div style={{ backgroundColor: C.white, border: `1px solid ${C.border}`, padding: '60px', textAlign: 'center' }}>
                                        <p style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.85rem', color: C.muted, letterSpacing: '2px', margin: 0 }}>
                                            Đang tải bộ sưu tập...
                                        </p>
                                    </div>
                                )}

                                {/* Error */}
                                {productsError && (
                                    <div style={{ backgroundColor: C.white, border: `1px solid #c0392b`, padding: '32px', textAlign: 'center' }}>
                                        <p style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.85rem', color: '#c0392b', margin: 0 }}>{productsError}</p>
                                    </div>
                                )}

                                {/* Không có sản phẩm */}
                                {!productsLoading && !productsError && filteredProducts.length === 0 && selectedCategory !== null && (
                                    <div style={{ backgroundColor: C.white, border: `1px solid ${C.border}`, padding: '60px', textAlign: 'center' }}>
                                        <p style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.85rem', color: C.muted, letterSpacing: '1px', margin: 0 }}>
                                            Danh mục này hiện chưa có sản phẩm phù hợp.
                                        </p>
                                    </div>
                                )}

                                {/* Grid sản phẩm */}
                                {!productsLoading && filteredProducts.length > 0 && (
                                    <div style={{
                                        display: 'grid',
                                        gridTemplateColumns: 'repeat(3, 1fr)',
                                        gap: '1px',
                                        backgroundColor: C.border,
                                        border: `1px solid ${C.border}`,
                                    }}>
                                        {filteredProducts.map(item => (
                                            <ProductCard
                                                key={item.id}
                                                item={item}
                                                onClick={() => navigate(`/product/${item.id}`)}
                                            />
                                        ))}
                                    </div>
                                )}
                            </>
                        )}
                    </div>
                </div>
            </section>

            {/* ══ STYLES ══ */}
            <style>{`
                @keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }
                .custom-cat-card:hover {
                    background-color: #f2ebd9 !important;
                    border-color: #b8975a !important;
                    box-shadow: 0 6px 16px rgba(184,151,90,0.15);
                    transform: translateY(-2px);
                }
                .custom-cat-card.active {
                    box-shadow: inset 0 0 0 2px #b8975a;
                }
            `}</style>
        </div>
    );
};

export default FashionPage;