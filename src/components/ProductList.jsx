/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : ProductList - Nhận thêm prop `gender` (nam/nu/treem/null)
               Lọc sản phẩm theo gender + categoryId, hiển thị badge giới tính
               [ĐÃ SỬA LỖI]: Hỗ trợ bóc tách chuỗi sizes dạng "10ml, 50ml, 100ml" tránh crash trang web
*/
import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import productService from '../services/productService';

const C = {
    gold: '#b8975a', cream: '#f7f3ee', white: '#ffffff',
    dark: '#1a1a1a', muted: '#8a8178', border: '#e4ddd4',
};

const GENDER_BADGE = {
    nam: { label: 'Nam', bg: '#1a1a1a', color: '#fff' },
    nu: { label: 'Nữ', bg: '#b8975a', color: '#fff' },
    treem: { label: 'Trẻ Em', bg: '#5a8ab8', color: '#fff' },
};

/* gender = 'nam' | 'nu' | 'treem' | null */
const ProductList = ({ categoryId, gender }) => {
    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchProducts = async () => {
            try {
                setLoading(true);
                let data;
                if (categoryId) {
                    data = await productService.getProductsByCategory(categoryId);
                } else {
                    data = await productService.getAllProducts();
                }
                setProducts(data.data || data || []);
            } catch (error) {
                console.error('Lỗi khi tải sản phẩm:', error);
            } finally {
                setLoading(false);
            }
        };
        fetchProducts();
    }, [categoryId]);

    /* Lọc theo gender phía client.
       Backend nên trả về field `gender: 'nam'|'nu'|'treem'` cho mỗi sản phẩm.
       Nếu backend chưa có field này, tất cả sản phẩm đều hiện (không lọc). */
    const displayed = gender
        ? products.filter(p => !p.gender || p.gender === gender)
        : products;

    if (loading) {
        return (
            <div style={{ textAlign: 'center', padding: '60px 0', fontFamily: "'Jost',sans-serif", fontSize: '0.85rem', color: C.muted, letterSpacing: '2px' }}>
                Đang tải bộ sưu tập...
            </div>
        );
    }

    if (displayed.length === 0) {
        return (
            <div style={{ textAlign: 'center', padding: '60px 0', backgroundColor: C.white, border: `1px solid ${C.border}` }}>
                <div style={{ fontSize: '2.5rem', marginBottom: '16px', opacity: 0.25 }}>👗</div>
                <p style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.82rem', color: C.muted, letterSpacing: '1.5px', textTransform: 'uppercase' }}>
                    {gender ? `Chưa có sản phẩm ${GENDER_BADGE[gender]?.label || ''}` : 'Danh mục này chưa có sản phẩm'}
                </p>
            </div>
        );
    }

    return (
        <div style={{
            display: 'grid',
            gridTemplateColumns: 'repeat(auto-fill, minmax(200px, 1fr))',
            gap: '1px',
            backgroundColor: C.border,
        }}>
            {displayed.map(item => (
                <div
                    key={item.id}
                    style={{ backgroundColor: C.white, display: 'flex', flexDirection: 'column', cursor: 'pointer', transition: 'background 0.2s' }}
                    onMouseEnter={e => e.currentTarget.style.backgroundColor = C.cream}
                    onMouseLeave={e => e.currentTarget.style.backgroundColor = C.white}
                    onClick={() => navigate(`/product/${item.id}`)}
                >
                    {/* Badges */}
                    <div style={{ padding: '14px 14px 0', display: 'flex', gap: '6px', minHeight: '28px', flexWrap: 'wrap' }}>
                        {/* Badge giới tính */}
                        {item.gender && GENDER_BADGE[item.gender] && (
                            <span style={{
                                fontFamily: "'Jost',sans-serif",
                                fontSize: '0.58rem',
                                letterSpacing: '1.5px',
                                textTransform: 'uppercase',
                                padding: '3px 7px',
                                backgroundColor: GENDER_BADGE[item.gender].bg,
                                color: GENDER_BADGE[item.gender].color,
                            }}>
                                {GENDER_BADGE[item.gender].label}
                            </span>
                        )}
                        {/* Badge trạng thái */}
                        {item.stockQuantity === 0 && (
                            <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.58rem', letterSpacing: '1.5px', textTransform: 'uppercase', padding: '3px 7px', backgroundColor: '#aaa', color: '#fff' }}>Hết hàng</span>
                        )}
                        {item.isNew && item.stockQuantity !== 0 && (
                            <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.58rem', letterSpacing: '1.5px', textTransform: 'uppercase', padding: '3px 7px', backgroundColor: C.dark, color: '#fff' }}>Mới</span>
                        )}
                        {item.isSale && (
                            <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.58rem', letterSpacing: '1.5px', textTransform: 'uppercase', padding: '3px 7px', backgroundColor: '#c0392b', color: '#fff' }}>Sale</span>
                        )}
                    </div>

                    {/* Ảnh */}
                    <div style={{ height: '180px', margin: '10px 14px', backgroundColor: C.cream, overflow: 'hidden', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                        <img
                            src={item.imageUrl || 'https://images.unsplash.com/photo-1523381210434-271e8be1f52b?q=80&w=300'}
                            alt={item.name}
                            style={{ width: '100%', height: '100%', objectFit: 'cover' }}
                        />
                    </div>

                    {/* Info */}
                    <div style={{ padding: '0 14px 18px', display: 'flex', flexDirection: 'column', flexGrow: 1 }}>
                        <h5 style={{ fontFamily: "'Cormorant Garamond',Georgia,serif", fontSize: '0.95rem', fontWeight: 400, color: C.dark, margin: '0 0 4px 0', lineHeight: 1.4 }}>
                            {item.name}
                        </h5>

                        {item.description && (
                            <p style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.72rem', color: C.muted, margin: '0 0 8px 0', fontWeight: 300, lineHeight: 1.5, overflow: 'hidden', display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical' }}>
                                {item.description}
                            </p>
                        )}

                        {/* Sizes - ĐÃ SỬA LỖI: Tự động phát hiện và chuyển đổi chuỗi thành mảng an toàn */}
                        {item.sizes && (
                            <div style={{ display: 'flex', gap: '4px', marginBottom: '10px', flexWrap: 'wrap' }}>
                                {(typeof item.sizes === 'string'
                                    ? item.sizes.split(',').map(s => s.trim())
                                    : Array.isArray(item.sizes) ? item.sizes : []
                                ).filter(s => s !== '').map((s, index) => (
                                    <span
                                        key={index}
                                        style={{
                                            fontFamily: "'Jost',sans-serif",
                                            fontSize: '0.6rem',
                                            letterSpacing: '1px',
                                            padding: '2px 6px',
                                            border: `1px solid ${C.border}`,
                                            color: C.muted
                                        }}
                                    >
                                        {s}
                                    </span>
                                ))}
                            </div>
                        )}

                        {/* Giá + Kho */}
                        <div style={{ marginTop: 'auto', paddingTop: '12px', borderTop: `1px solid ${C.border}`, display: 'flex', justifyContent: 'space-between', alignItems: 'flex-end' }}>
                            <div>
                                {item.originalPrice && item.originalPrice > item.price && (
                                    <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.72rem', color: C.muted, textDecoration: 'line-through', display: 'block', fontWeight: 300 }}>
                                        {item.originalPrice.toLocaleString('vi-VN')}₫
                                    </span>
                                )}
                                <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.9rem', fontWeight: 500, color: C.dark }}>
                                    {item.price ? item.price.toLocaleString('vi-VN') + ' ₫' : 'Liên hệ'}
                                </span>
                                {item.stockQuantity !== undefined && (
                                    <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.67rem', color: C.muted, display: 'block', fontWeight: 300, marginTop: '2px' }}>
                                        Kho: <strong style={{ color: C.dark }}>{item.stockQuantity}</strong>
                                    </span>
                                )}
                            </div>
                            <button
                                onClick={e => { e.stopPropagation(); navigate(`/product/${item.id}`); }}
                                style={{
                                    fontFamily: "'Jost',sans-serif",
                                    fontSize: '0.65rem',
                                    letterSpacing: '1px',
                                    textTransform: 'uppercase',
                                    padding: '7px 12px',
                                    backgroundColor: C.dark,
                                    color: '#fff',
                                    border: 'none',
                                    cursor: 'pointer',
                                    fontWeight: 500,
                                    transition: 'background 0.2s',
                                }}
                                onMouseEnter={e => e.currentTarget.style.backgroundColor = C.gold}
                                onMouseLeave={e => e.currentTarget.style.backgroundColor = C.dark}
                            >
                                Xem →
                            </button>
                        </div>
                    </div>
                </div>
            ))}
        </div>
    );
};

export default ProductList;