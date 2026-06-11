/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Shop.jsx - Redesign theo phong cách Mai Trinh Studio
               [SỬA LỖI ĐẾM ALL = 0]: Tự động lấy toàn bộ sản phẩm thuộc các danh mục 
               đang hiển thị để tính tổng chuẩn xác 100%.
*/
import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import productService from '../api/productService';

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

const GENDERS = [
    { key: 'nam', icon: '👔', label: 'NAM', sub: 'Lịch lãm & hiện đại' },
    { key: 'nu', icon: '👗', label: 'NỮ', sub: 'Tinh tế & quyến rũ' },
    { key: 'treem', icon: '🧒', label: 'TRẺ EM', sub: 'Năng động & thoải mái' },
];

const Shop = ({ type = 'thoi-trang' }) => {
    const [categories, setCategories] = useState([]);
    const [selectedCategory, setSelectedCategory] = useState(null);
    const [products, setProducts] = useState([]);
    const [productsLoading, setProductsLoading] = useState(false);
    const [productsError, setProductsError] = useState(null);
    const [selectedSize, setSelectedSize] = useState(null);
    const [currentPage, setCurrentPage] = useState(1);
    const ITEMS_PER_PAGE = 10;

    const SIZES = type === 'nuoc-hoa' ? ['10ml', '50ml', '100ml', '125ml'] : ['S', 'M', 'L', 'XL', 'XXL'];

    const navigate = useNavigate();
    const PAGE_CONFIG = {
        'thoi-trang': { title: 'Thời Trang', sub: 'Thời trang cao cấp — dành cho mọi phong cách', icon: '👗', breadcrumb: 'Thời Trang', showGenders: true },
        'nuoc-hoa': { title: 'Nước Hoa', sub: 'Hương thơm tinh tế — đẳng cấp quý phái', icon: '🌸', breadcrumb: 'Nước Hoa', showGenders: false },
        'my-pham': { title: 'Mỹ Phẩm', sub: 'Làm đẹp tự nhiên — chăm sóc toàn diện', icon: '💄', breadcrumb: 'Mỹ Phẩm', showGenders: false },
        'phu-kien': { title: 'Phụ Kiện', sub: 'Điểm nhấn hoàn hảo — phong cách riêng', icon: '👜', breadcrumb: 'Phụ Kiện', showGenders: false },
    };
    const config = PAGE_CONFIG[type] || PAGE_CONFIG['thoi-trang'];

    useEffect(() => {
        const fetchMainCategories = async () => {
            try {
                const res = await productService.getCategories();
                const all = res.data || res;
                const keywords = {
                    'thoi-trang': ['thời trang', 'áo', 'quần', 'váy', 'đầm', 'set bộ', 'chân váy', 'giày dép', 'đồ lót', 'thể thao', 'phong cách'],
                    'nuoc-hoa': ['nước hoa'],
                    'my-pham': ['mỹ phẩm', 'son', 'kem', 'serum'],
                    'phu-kien': ['phụ kiện', 'túi', 'ví', 'thắt lưng', 'đồng hồ'],
                };
                const filtered = all.filter(cat =>
                    (keywords[type] || []).some(kw =>
                        cat.name?.toLowerCase().includes(kw.toLowerCase())
                    )
                );
                setCategories(filtered);
            } catch (err) {
                console.error('Lỗi khi tải danh mục:', err);
            }
        };
        fetchMainCategories();
        setSelectedCategory(null);
        setSelectedSize(null);
        setCurrentPage(1);
        setProducts([]);
    }, [type]);

    useEffect(() => {
        const fetchProductsData = async () => {
            if (categories.length === 0) return;
            try {
                setProductsLoading(true);
                setProductsError(null);
                if (selectedCategory === null) {
                    const res = await productService.getAllProducts();
                    const allProds = res.data || res;
                    const validCategoryIds = categories.map(c => c.id);
                    const tabProducts = allProds.filter(p =>
                        validCategoryIds.includes(p.categoryId) ||
                        validCategoryIds.includes(p.category_id)
                    );
                    if (tabProducts.length === 0 && type === 'nuoc-hoa') {
                        const fallbackPerfumes = allProds.filter(p =>
                            p.name?.toLowerCase().includes('nước hoa') ||
                            p.categoryName?.toLowerCase().includes('nước hoa') ||
                            p.description?.toLowerCase().includes('nước hoa')
                        );
                        setProducts(fallbackPerfumes.length > 0 ? fallbackPerfumes : allProds);
                    } else {
                        setProducts(tabProducts);
                    }
                } else {
                    const res = await productService.getProductsByCategory(selectedCategory);
                    let data = res.data || res;
                    if ((!data || data.length === 0) && type === 'nuoc-hoa') {
                        const currentCat = categories.find(c => c.id === selectedCategory);
                        if (currentCat) {
                            const allProdsRes = await productService.getAllProducts();
                            const allProds = allProdsRes.data || allProdsRes;
                            if (currentCat.name?.toLowerCase().includes('nữ')) {
                                data = allProds.filter(p =>
                                    p.name?.toLowerCase().includes('nữ') ||
                                    p.categoryName?.toLowerCase().includes('nữ')
                                );
                            } else if (currentCat.name?.toLowerCase().includes('nam')) {
                                data = allProds.filter(p =>
                                    p.name?.toLowerCase().includes('nam') ||
                                    p.categoryName?.toLowerCase().includes('nam')
                                );
                            }
                        }
                    }
                    setProducts(data || []);
                }
            } catch (err) {
                console.error('Lỗi hệ thống khi tải sản phẩm:', err);
                setProductsError('Hệ thống kết nối API đang bận. Vui lòng kiểm tra lại Backend!');
            } finally {
                setProductsLoading(false);
            }
        };
        fetchProductsData();
    }, [selectedCategory, type, categories]);

    const filteredProducts = selectedSize
        ? products.filter(p => {
            if (!p.sizes) return false;
            const itemSizesArray = typeof p.sizes === 'string'
                ? p.sizes.split(',').map(s => s.trim())
                : Array.isArray(p.sizes) ? p.sizes : [];
            return itemSizesArray.includes(selectedSize);
        })
        : products;

    const totalPages = Math.ceil(filteredProducts.length / ITEMS_PER_PAGE);
    const pagedProducts = filteredProducts.slice(
        (currentPage - 1) * ITEMS_PER_PAGE,
        currentPage * ITEMS_PER_PAGE
    );

    const CategoryBtn = ({ active, onClick, children }) => (
        <button
            onClick={onClick}
            style={{
                fontFamily: "'Jost', sans-serif",
                fontSize: '0.75rem',
                letterSpacing: '1.5px',
                textTransform: 'uppercase',
                fontWeight: 500,
                padding: '10px 22px',
                border: `1px solid ${active ? C.dark : C.border}`,
                backgroundColor: active ? C.dark : 'transparent',
                color: active ? '#fff' : C.muted,
                cursor: 'pointer',
                borderRadius: '2px',
                transition: 'all 0.2s',
            }}
            onMouseEnter={e => { if (!active) { e.currentTarget.style.borderColor = C.gold; e.currentTarget.style.color = C.gold; } }}
            onMouseLeave={e => { if (!active) { e.currentTarget.style.borderColor = C.border; e.currentTarget.style.color = C.muted; } }}
        >
            {children}
        </button>
    );

    const SizeBtn = ({ active, onClick, children }) => (
        <button
            onClick={onClick}
            style={{
                fontFamily: "'Jost', sans-serif",
                fontSize: '0.72rem',
                letterSpacing: '1px',
                fontWeight: 500,
                minWidth: '44px',
                height: '44px',
                padding: '0 8px',
                border: `1px solid ${active ? C.gold : C.border}`,
                backgroundColor: active ? C.gold : 'transparent',
                color: active ? '#fff' : C.muted,
                cursor: 'pointer',
                borderRadius: '2px',
                transition: 'all 0.2s',
            }}
        >
            {children}
        </button>
    );

    return (
        <div style={{ fontFamily: "'Cormorant Garamond', 'Playfair Display', Georgia, serif", backgroundColor: C.cream, padding: '0 0 60px 0' }}>
            <link href="https://fonts.googleapis.com/css2?family=Cormorant+Garamond:ital,wght@0,300;0,400;0,600;1,400&family=Jost:wght@300;400;500&display=swap" rel="stylesheet" />

            {/* BREADCRUMB */}
            <div style={{ backgroundColor: C.white, borderBottom: `1px solid ${C.border}`, padding: '10px 40px' }}>
                <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.72rem', color: C.muted, letterSpacing: '0.5px' }}>
                    <span onClick={() => navigate('/')} style={{ cursor: 'pointer', color: C.muted }} onMouseEnter={e => e.target.style.color = C.gold} onMouseLeave={e => e.target.style.color = C.muted}>
                        🏠 Trang Chủ
                    </span>
                    <span style={{ margin: '0 8px', opacity: 0.4 }}>›</span>
                    <span style={{ color: C.dark, fontWeight: 500 }}>{config.breadcrumb}</span>
                </span>
            </div>

            {/* PAGE TITLE */}
            <div style={{ backgroundColor: C.white, borderBottom: `1px solid ${C.border}`, padding: '32px 40px', marginBottom: '32px' }}>
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: '24px' }}>
                    <div>
                        <p style={{ fontFamily: "'Jost', sans-serif", fontSize: '0.7rem', letterSpacing: '3px', color: C.gold, textTransform: 'uppercase', margin: '0 0 8px 0', fontWeight: 500 }}>
                            BỘ SƯU TẬP 2026
                        </p>
                        <h1 style={{ fontSize: '2rem', fontWeight: 300, color: C.dark, margin: '0 0 8px 0' }}>
                            {config.title} <em style={{ color: C.gold, fontStyle: 'italic' }}>{config.icon}</em>
                        </h1>
                        <p style={{ fontFamily: "'Jost', sans-serif", fontSize: '0.85rem', color: C.muted, margin: 0, fontWeight: 300 }}>
                            {config.sub}
                        </p>
                    </div>
                </div>

                {config.showGenders && (
                    <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: '12px' }}>
                        {GENDERS.map(g => (
                            <div key={g.key} onClick={() => navigate(`/thoi-trang/${g.key}`)}
                                style={{ padding: '20px', border: `1px solid ${C.border}`, backgroundColor: C.cream, textAlign: 'center', cursor: 'pointer', transition: 'all 0.2s', display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '8px' }}
                                onMouseEnter={e => { e.currentTarget.style.borderColor = C.gold; e.currentTarget.style.backgroundColor = '#f2ebd9'; }}
                                onMouseLeave={e => { e.currentTarget.style.borderColor = C.border; e.currentTarget.style.backgroundColor = C.cream; }}
                            >
                                <div style={{ fontSize: '2rem' }}>{g.icon}</div>
                                <p style={{ fontFamily: "'Jost', sans-serif", fontSize: '0.75rem', letterSpacing: '2px', textTransform: 'uppercase', fontWeight: 600, color: C.dark, margin: 0 }}>{g.label}</p>
                                <p style={{ fontFamily: "'Jost', sans-serif", fontSize: '0.68rem', color: C.muted, margin: 0, fontWeight: 300 }}>{g.sub}</p>
                            </div>
                        ))}
                    </div>
                )}
            </div>

            <div style={{ maxWidth: '1400px', margin: '0 auto', padding: '0 24px' }}>

                {/* DANH MỤC */}
                <div style={{ marginBottom: '24px' }}>
                    <p style={{ fontFamily: "'Jost', sans-serif", fontSize: '0.68rem', letterSpacing: '3px', color: C.muted, textTransform: 'uppercase', marginBottom: '14px', fontWeight: 500 }}>
                        DANH MỤC {config.title.toUpperCase()}
                    </p>
                    <div style={{ display: 'flex', flexWrap: 'wrap', gap: '8px' }}>
                        <CategoryBtn active={selectedCategory === null} onClick={() => { setSelectedCategory(null); setSelectedSize(null); setCurrentPage(1); }}>
                            Tất Cả
                        </CategoryBtn>
                        {categories.map(cat => (
                            <CategoryBtn key={cat.id} active={selectedCategory === cat.id} onClick={() => { setSelectedCategory(cat.id); setSelectedSize(null); setCurrentPage(1); }}>
                                {cat.name}
                            </CategoryBtn>
                        ))}
                    </div>
                </div>

                {/* BỘ LỌC SIZE */}
                <div style={{ backgroundColor: C.white, border: `1px solid ${C.border}`, padding: '20px 24px', marginBottom: '24px', display: 'flex', alignItems: 'center', gap: '16px' }}>
                    <span style={{ fontFamily: "'Jost', sans-serif", fontSize: '0.7rem', letterSpacing: '2px', color: C.muted, textTransform: 'uppercase', fontWeight: 500, whiteSpace: 'nowrap' }}>
                        {type === 'nuoc-hoa' ? 'DUNG TÍCH' : 'KÍCH CỠ'}
                    </span>
                    <div style={{ width: '1px', height: '24px', backgroundColor: C.border }} />
                    <div style={{ display: 'flex', gap: '8px' }}>
                        <SizeBtn active={selectedSize === null} onClick={() => { setSelectedSize(null); setCurrentPage(1); }}>ALL</SizeBtn>
                        {SIZES.map(size => (
                            <SizeBtn key={size} active={selectedSize === size} onClick={() => { setSelectedSize(size); setCurrentPage(1); }}>{size}</SizeBtn>
                        ))}
                    </div>
                </div>

                {/* GRID SẢN PHẨM */}
                <div style={{ backgroundColor: C.white, border: `1px solid ${C.border}`, padding: '32px', minHeight: '300px' }}>
                    <div style={{ display: 'flex', alignItems: 'baseline', justifyContent: 'space-between', marginBottom: '28px', borderBottom: `1px solid ${C.border}`, paddingBottom: '16px' }}>
                        <h2 style={{ fontSize: '1.1rem', fontWeight: 400, color: C.dark, margin: 0, letterSpacing: '1px' }}>
                            Sản Phẩm {config.title}
                            {filteredProducts.length > 0 && (
                                <span style={{ fontFamily: "'Jost', sans-serif", fontSize: '0.78rem', color: C.muted, fontWeight: 300, marginLeft: '12px' }}>
                                    — {filteredProducts.length} sản phẩm
                                </span>
                            )}
                        </h2>
                    </div>

                    {productsLoading && (
                        <div style={{ textAlign: 'center', padding: '60px 0' }}>
                            <div style={{ fontFamily: "'Jost', sans-serif", fontSize: '0.85rem', color: C.muted, letterSpacing: '2px' }}>Đang tải bộ sưu tập...</div>
                        </div>
                    )}

                    {productsError && (
                        <div style={{ textAlign: 'center', padding: '40px', fontFamily: "'Jost', sans-serif", fontSize: '0.85rem', color: '#c0392b' }}>{productsError}</div>
                    )}

                    {!productsLoading && filteredProducts.length === 0 && !productsError && (
                        <div style={{ textAlign: 'center', padding: '60px 0' }}>
                            <img src="https://cdn-icons-png.flaticon.com/512/4076/4076478.png" alt="Không tìm thấy sản phẩm" style={{ width: '120px', opacity: 0.35, marginBottom: '20px' }} />
                            <p style={{ fontFamily: "'Jost', sans-serif", fontSize: '0.85rem', color: C.muted, letterSpacing: '1px' }}>
                                Không tìm thấy sản phẩm nào phù hợp với tiêu chí của bạn.
                            </p>
                        </div>
                    )}

                    {!productsLoading && pagedProducts.length > 0 && (
                        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(5, 1fr)', gap: '1px', backgroundColor: C.border }}>
                            {pagedProducts.map(item => (
                                <div key={item.id}
                                    style={{ backgroundColor: C.white, padding: '20px', display: 'flex', flexDirection: 'column', cursor: 'pointer', transition: 'background 0.2s' }}
                                    onMouseEnter={e => e.currentTarget.style.backgroundColor = C.cream}
                                    onMouseLeave={e => e.currentTarget.style.backgroundColor = C.white}
                                    onClick={() => navigate(`/product/${item.id}`)}
                                >
                                    <div style={{ marginBottom: '12px', minHeight: '20px' }}>
                                        {item.isNew && <span style={{ fontFamily: "'Jost', sans-serif", fontSize: '0.6rem', letterSpacing: '2px', textTransform: 'uppercase', backgroundColor: C.dark, color: '#fff', padding: '3px 8px', marginRight: '4px' }}>Mới</span>}
                                        {item.isSale && <span style={{ fontFamily: "'Jost', sans-serif", fontSize: '0.6rem', letterSpacing: '2px', textTransform: 'uppercase', backgroundColor: '#c0392b', color: '#fff', padding: '3px 8px', marginRight: '4px' }}>Sale</span>}
                                        {item.isHot && <span style={{ fontFamily: "'Jost', sans-serif", fontSize: '0.6rem', letterSpacing: '2px', textTransform: 'uppercase', backgroundColor: C.gold, color: '#fff', padding: '3px 8px' }}>Hot</span>}
                                    </div>
                                    <div style={{ height: '200px', backgroundColor: C.cream, display: 'flex', alignItems: 'center', justifyContent: 'center', marginBottom: '16px', overflow: 'hidden' }}>
                                        <img src={item.imageUrl || 'https://images.unsplash.com/photo-1523381210434-271e8be1f52b?q=80&w=300'} alt={item.name} style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
                                    </div>
                                    <h5 style={{ fontSize: '0.88rem', fontWeight: 400, color: C.dark, margin: '0 0 4px 0', lineHeight: 1.4 }}>{item.name}</h5>
                                    {item.categoryName && <p style={{ fontFamily: "'Jost', sans-serif", fontSize: '0.72rem', color: C.muted, margin: '0 0 10px 0', letterSpacing: '0.5px', fontWeight: 300 }}>{item.categoryName}</p>}
                                    {item.sizes && (
                                        <div style={{ display: 'flex', gap: '4px', marginBottom: '10px', flexWrap: 'wrap' }}>
                                            {(typeof item.sizes === 'string' ? item.sizes.split(',').map(s => s.trim()) : Array.isArray(item.sizes) ? item.sizes : []).filter(s => s !== '').map((s, index) => (
                                                <span key={index} style={{ fontFamily: "'Jost', sans-serif", fontSize: '0.62rem', letterSpacing: '1px', padding: '2px 6px', border: `1px solid ${C.border}`, color: C.muted }}>{s}</span>
                                            ))}
                                        </div>
                                    )}
                                    <div style={{ marginTop: 'auto', paddingTop: '12px' }}>
                                        {item.originalPrice && item.originalPrice > item.price && (
                                            <span style={{ fontFamily: "'Jost', sans-serif", fontSize: '0.75rem', color: C.muted, textDecoration: 'line-through', marginRight: '8px', fontWeight: 300 }}>
                                                {item.originalPrice.toLocaleString('vi-VN')}₫
                                            </span>
                                        )}
                                        <span style={{ fontFamily: "'Jost', sans-serif", fontSize: '0.9rem', fontWeight: 500, color: C.dark, letterSpacing: '0.5px' }}>
                                            {item.price ? item.price.toLocaleString('vi-VN') + ' ₫' : 'Liên hệ'}
                                        </span>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}

                    {/* PHÂN TRANG */}
                    {totalPages > 1 && (
                        <div style={{ display: 'flex', justifyContent: 'center', gap: '6px', marginTop: '32px', paddingTop: '24px', borderTop: `1px solid ${C.border}` }}>
                            <button onClick={() => setCurrentPage(1)} disabled={currentPage === 1}
                                style={{ fontFamily: "'Jost', sans-serif", fontSize: '0.75rem', padding: '8px 14px', border: `1px solid ${C.border}`, backgroundColor: 'transparent', color: currentPage === 1 ? C.border : C.muted, cursor: currentPage === 1 ? 'default' : 'pointer', borderRadius: '2px' }}>
                                First
                            </button>
                            <button onClick={() => setCurrentPage(p => Math.max(1, p - 1))} disabled={currentPage === 1}
                                style={{ fontFamily: "'Jost', sans-serif", fontSize: '0.75rem', padding: '8px 14px', border: `1px solid ${C.border}`, backgroundColor: 'transparent', color: currentPage === 1 ? C.border : C.muted, cursor: currentPage === 1 ? 'default' : 'pointer', borderRadius: '2px' }}>
                                Previous
                            </button>
                            {Array.from({ length: totalPages }, (_, i) => i + 1)
                                .filter(page => page === 1 || page === totalPages || (page >= currentPage - 2 && page <= currentPage + 2))
                                .reduce((acc, page, idx, arr) => {
                                    if (idx > 0 && page - arr[idx - 1] > 1) acc.push('...');
                                    acc.push(page);
                                    return acc;
                                }, [])
                                .map((page, idx) => page === '...' ? (
                                    <span key={`dot-${idx}`} style={{ fontFamily: "'Jost', sans-serif", fontSize: '0.75rem', padding: '8px 6px', color: C.muted }}>...</span>
                                ) : (
                                    <button key={page} onClick={() => setCurrentPage(page)}
                                        style={{ fontFamily: "'Jost', sans-serif", fontSize: '0.75rem', padding: '8px 12px', border: `1px solid ${page === currentPage ? C.dark : C.border}`, backgroundColor: page === currentPage ? C.dark : 'transparent', color: page === currentPage ? '#fff' : C.muted, cursor: 'pointer', borderRadius: '2px', fontWeight: page === currentPage ? 500 : 400 }}>
                                        {page}
                                    </button>
                                ))
                            }
                            <button onClick={() => setCurrentPage(p => Math.min(totalPages, p + 1))} disabled={currentPage === totalPages}
                                style={{ fontFamily: "'Jost', sans-serif", fontSize: '0.75rem', padding: '8px 14px', border: `1px solid ${C.border}`, backgroundColor: 'transparent', color: currentPage === totalPages ? C.border : C.muted, cursor: currentPage === totalPages ? 'default' : 'pointer', borderRadius: '2px' }}>
                                Next
                            </button>
                            <button onClick={() => setCurrentPage(totalPages)} disabled={currentPage === totalPages}
                                style={{ fontFamily: "'Jost', sans-serif", fontSize: '0.75rem', padding: '8px 14px', border: `1px solid ${C.border}`, backgroundColor: 'transparent', color: currentPage === totalPages ? C.border : C.muted, cursor: currentPage === totalPages ? 'default' : 'pointer', borderRadius: '2px' }}>
                                Last
                            </button>
                        </div>
                    )}

                </div>
            </div>
        </div>
    );
};

export default Shop;