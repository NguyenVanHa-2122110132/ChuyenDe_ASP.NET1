/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Cập nhật : 07/06/2026
    Mô tả    : CategoryProductList - Lọc danh mục theo nhóm (thoi-trang / nuoc-hoa / my-pham / phu-kien)
*/
import React, { useState, useEffect } from 'react';
import categoryProductService from '../services/categoryProductService';

const C = {
    gold: '#b8975a', cream: '#f7f3ee', white: '#ffffff',
    dark: '#1a1a1a', muted: '#8a8178', border: '#e4ddd4',
};

const GENDER_INFO = {
    nam: { label: 'Nam', icon: '👔', color: '#1a1a1a' },
    nu: { label: 'Nữ', icon: '👗', color: '#b8975a' },
    treem: { label: 'Trẻ Em', icon: '🧒', color: '#5a8ab8' },
};

// Từ khóa lọc danh mục theo nhóm
const CATEGORY_KEYWORDS = {
    'thoi-trang': ['thời trang', 'áo', 'quần', 'váy', 'đầm', 'set bộ', 'chân váy', 'giày dép', 'phụ kiện', 'đồ lót', 'thể thao', 'phong cách'],
    'nuoc-hoa': ['nước hoa'],
    'my-pham': ['mỹ phẩm', 'son', 'kem', 'serum', 'toner', 'makeup'],
    'phu-kien': ['phụ kiện', 'túi', 'ví', 'thắt lưng', 'đồng hồ'],
};

const CategoryProductList = ({ selectedId, onSelectCategory, gender, categoryType = 'thoi-trang' }) => {
    const [categoryProducts, setCategoryProducts] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchCategoryProducts = async () => {
            try {
                setLoading(true);
                const data = await categoryProductService.getAllCategoryProducts();
                const all = data.data || data;

                // Lọc danh mục theo nhóm
                const keywords = CATEGORY_KEYWORDS[categoryType] || [];
                const filtered = all.filter(item =>
                    keywords.some(kw =>
                        item.name?.toLowerCase().includes(kw.toLowerCase())
                    )
                );

                setCategoryProducts(filtered);
            } catch (error) {
                console.error('Lỗi khi tải danh mục:', error);
            } finally {
                setLoading(false);
            }
        };
        fetchCategoryProducts();
    }, [categoryType]);

    if (loading) {
        return (
            <div style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.75rem', color: C.muted, padding: '16px 0', letterSpacing: '1px' }}>
                Đang tải danh mục...
            </div>
        );
    }

    return (
        <div style={{ backgroundColor: C.white, border: `1px solid ${C.border}` }}>
            {/* Header sidebar */}
            <div style={{ padding: '18px 20px 14px', borderBottom: `1px solid ${C.border}` }}>
                <p style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.65rem', letterSpacing: '3px', color: C.muted, textTransform: 'uppercase', margin: 0, fontWeight: 500 }}>
                    DANH MỤC SP
                </p>
                {gender && GENDER_INFO[gender] && (
                    <div style={{ marginTop: '10px', display: 'flex', alignItems: 'center', gap: '8px' }}>
                        <span style={{ fontSize: '1rem' }}>{GENDER_INFO[gender].icon}</span>
                        <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.72rem', letterSpacing: '1.5px', color: GENDER_INFO[gender].color, textTransform: 'uppercase', fontWeight: 600 }}>
                            Thời Trang {GENDER_INFO[gender].label}
                        </span>
                    </div>
                )}
            </div>

            {/* Danh sách danh mục */}
            <div>
                {categoryProducts.length === 0 ? (
                    <div style={{ padding: '20px', textAlign: 'center', fontFamily: "'Jost',sans-serif", fontSize: '0.75rem', color: C.muted }}>
                        Không có danh mục nào.
                    </div>
                ) : (
                    categoryProducts.map(item => (
                        <button
                            key={item.id}
                            onClick={() => onSelectCategory(item.id)}
                            style={{
                                width: '100%',
                                padding: '12px 20px',
                                display: 'flex',
                                justifyContent: 'space-between',
                                alignItems: 'center',
                                borderBottom: `1px solid ${C.border}`,
                                border: 'none',
                                backgroundColor: selectedId === item.id ? C.dark : 'transparent',
                                cursor: 'pointer',
                                transition: 'all 0.15s',
                            }}
                            onMouseEnter={e => { if (selectedId !== item.id) e.currentTarget.style.backgroundColor = C.cream; }}
                            onMouseLeave={e => { if (selectedId !== item.id) e.currentTarget.style.backgroundColor = 'transparent'; }}
                        >
                            <span style={{
                                fontFamily: "'Jost',sans-serif",
                                fontSize: '0.78rem',
                                letterSpacing: '0.5px',
                                color: selectedId === item.id ? '#fff' : C.dark,
                                fontWeight: selectedId === item.id ? 500 : 300,
                            }}>
                                {item.name}
                            </span>
                            <span style={{ fontSize: '0.6rem', color: selectedId === item.id ? '#fff' : C.muted, opacity: 0.6 }}>›</span>
                        </button>
                    ))
                )}
            </div>
        </div>
    );
};

export default CategoryProductList;