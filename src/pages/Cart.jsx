/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 09/06/2026
    Mô tả    : Trang Giỏ Hàng (Cart.jsx)
              - Đọc/ghi giỏ hàng từ localStorage
              - Tăng/giảm số lượng, xóa sản phẩm
              - Cảnh báo vượt StockQuantity
              - Tính tổng tiền trước khi chuyển sang Checkout
*/
import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';

const C = {
    gold: '#b8975a',
    cream: '#f7f3ee',
    creamDark: '#ede8e0',
    white: '#ffffff',
    dark: '#1a1a1a',
    muted: '#8a8178',
    border: '#e4ddd4',
    danger: '#c0392b',
};

// ── HELPER: đọc giỏ hàng từ localStorage ──
export function getCart() {
    try {
        return JSON.parse(localStorage.getItem('cart') || '[]');
    } catch {
        return [];
    }
}

// ── HELPER: lưu giỏ hàng vào localStorage ──
export function saveCart(items) {
    localStorage.setItem('cart', JSON.stringify(items));
    // Phát sự kiện để Header cập nhật badge số lượng
    window.dispatchEvent(new Event('cartUpdated'));
}

// ── HELPER: thêm sản phẩm vào giỏ ──
export function addToCart(product, quantity = 1) {
    const cart = getCart();
    const existing = cart.find(i => i.id === product.id);
    if (existing) {
        const newQty = existing.quantity + quantity;
        existing.quantity = Math.min(newQty, product.stock || 99);
    } else {
        cart.push({
            id: product.id,
            name: product.name,
            price: product.price,
            imageUrl: product.imageUrl,
            stock: product.stock || 99,
            quantity,
        });
    }
    saveCart(cart);
}

export default function CartPage() {
    const navigate = useNavigate();
    const [cartItems, setCartItems] = useState([]);

    useEffect(() => {
        setCartItems(getCart());
    }, []);

    // Thay đổi số lượng — kiểm tra vượt tồn kho
    const handleQtyChange = (id, delta) => {
        const updated = cartItems.map(item => {
            if (item.id !== id) return item;
            const newQty = item.quantity + delta;
            if (newQty < 1) return item;
            if (newQty > item.stock) {
                alert(`⚠️ Số lượng sản phẩm trong kho chỉ còn ${item.stock}!`);
                return item;
            }
            return { ...item, quantity: newQty };
        });
        setCartItems(updated);
        saveCart(updated);
    };

    // Xóa 1 sản phẩm
    const handleRemove = (id) => {
        const updated = cartItems.filter(i => i.id !== id);
        setCartItems(updated);
        saveCart(updated);
    };

    // Xóa toàn bộ giỏ hàng
    const handleClearCart = () => {
        if (window.confirm('Bạn có chắc muốn xóa toàn bộ giỏ hàng?')) {
            setCartItems([]);
            saveCart([]);
        }
    };

    const total = cartItems.reduce((sum, i) => sum + i.price * i.quantity, 0);
    const totalItems = cartItems.reduce((sum, i) => sum + i.quantity, 0);

    return (
        <div style={{ backgroundColor: C.cream, minHeight: '100vh', fontFamily: "'Cormorant Garamond','Playfair Display',Georgia,serif" }}>
            <link href="https://fonts.googleapis.com/css2?family=Cormorant+Garamond:ital,wght@0,300;0,400;0,600;1,400&family=Jost:wght@300;400;500&display=swap" rel="stylesheet" />

            {/* Breadcrumb */}
            <div style={{ backgroundColor: C.white, borderBottom: `1px solid ${C.border}`, padding: '10px 40px' }}>
                <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.72rem', color: C.muted }}>
                    <Link to="/" style={{ color: C.muted, textDecoration: 'none' }}>🏠 Trang Chủ</Link>
                    <span style={{ margin: '0 8px', opacity: 0.4 }}>›</span>
                    <span style={{ color: C.dark, fontWeight: 500 }}>Giỏ Hàng</span>
                </span>
            </div>

            <div style={{ maxWidth: '1100px', margin: '0 auto', padding: '40px 24px' }}>
                <h1 style={{ fontSize: '1.8rem', fontWeight: 300, color: C.dark, marginBottom: '8px' }}>
                    Giỏ Hàng <em style={{ color: C.gold, fontStyle: 'italic' }}>của bạn</em>
                </h1>
                <p style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.8rem', color: C.muted, marginBottom: '32px', fontWeight: 300 }}>
                    {totalItems > 0 ? `${totalItems} sản phẩm` : 'Chưa có sản phẩm nào'}
                </p>

                {/* Giỏ trống */}
                {cartItems.length === 0 && (
                    <div style={{ backgroundColor: C.white, border: `1px solid ${C.border}`, padding: '60px', textAlign: 'center' }}>
                        <div style={{ fontSize: '3rem', marginBottom: '16px' }}>🛒</div>
                        <p style={{ fontFamily: "'Jost',sans-serif", color: C.muted, fontSize: '0.9rem', marginBottom: '24px' }}>
                            Giỏ hàng của bạn đang trống.
                        </p>
                        <Link to="/" style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.78rem', letterSpacing: '2px', fontWeight: 500, textTransform: 'uppercase', padding: '12px 28px', border: `1px solid ${C.dark}`, color: C.dark, textDecoration: 'none', backgroundColor: 'transparent' }}>
                            TIẾP TỤC MUA SẮM
                        </Link>
                    </div>
                )}

                {/* Có sản phẩm */}
                {cartItems.length > 0 && (
                    <div style={{ display: 'grid', gridTemplateColumns: '1fr 320px', gap: '24px', alignItems: 'start' }}>

                        {/* Danh sách sản phẩm */}
                        <div style={{ backgroundColor: C.white, border: `1px solid ${C.border}` }}>
                            {/* Header bảng */}
                            <div style={{ display: 'grid', gridTemplateColumns: '1fr 100px 120px 80px 40px', gap: '12px', padding: '14px 24px', borderBottom: `1px solid ${C.border}`, backgroundColor: C.creamDark }}>
                                {['Sản phẩm', 'Đơn giá', 'Số lượng', 'Thành tiền', ''].map(h => (
                                    <span key={h} style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.68rem', letterSpacing: '1.5px', textTransform: 'uppercase', color: C.muted, fontWeight: 500 }}>{h}</span>
                                ))}
                            </div>

                            {cartItems.map(item => (
                                <div key={item.id} style={{ display: 'grid', gridTemplateColumns: '1fr 100px 120px 80px 40px', gap: '12px', padding: '20px 24px', borderBottom: `1px solid ${C.border}`, alignItems: 'center' }}>
                                    {/* Tên + ảnh */}
                                    <div style={{ display: 'flex', gap: '14px', alignItems: 'center' }}>
                                        <img src={item.imageUrl || 'https://images.unsplash.com/photo-1523381210434-271e8be1f52b?q=80&w=80'} alt={item.name}
                                            style={{ width: '60px', height: '60px', objectFit: 'cover', border: `1px solid ${C.border}` }} />
                                        <span style={{ fontSize: '0.88rem', fontWeight: 400, color: C.dark, lineHeight: 1.4 }}>{item.name}</span>
                                    </div>

                                    {/* Đơn giá */}
                                    <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.85rem', color: C.dark }}>
                                        {item.price.toLocaleString('vi-VN')}₫
                                    </span>

                                    {/* Số lượng */}
                                    <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                                        <button onClick={() => handleQtyChange(item.id, -1)}
                                            style={{ width: '28px', height: '28px', border: `1px solid ${C.border}`, background: 'transparent', cursor: 'pointer', fontSize: '1rem', color: C.dark, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>−</button>
                                        <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.9rem', minWidth: '24px', textAlign: 'center', color: C.dark }}>{item.quantity}</span>
                                        <button onClick={() => handleQtyChange(item.id, 1)}
                                            style={{ width: '28px', height: '28px', border: `1px solid ${C.border}`, background: 'transparent', cursor: 'pointer', fontSize: '1rem', color: C.dark, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>+</button>
                                    </div>

                                    {/* Thành tiền */}
                                    <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.88rem', fontWeight: 500, color: C.gold }}>
                                        {(item.price * item.quantity).toLocaleString('vi-VN')}₫
                                    </span>

                                    {/* Xóa */}
                                    <button onClick={() => handleRemove(item.id)}
                                        style={{ background: 'none', border: 'none', cursor: 'pointer', color: C.muted, fontSize: '1rem', padding: 0, lineHeight: 1 }}
                                        title="Xóa sản phẩm">✕</button>
                                </div>
                            ))}

                            {/* Nút xóa tất cả */}
                            <div style={{ padding: '14px 24px', display: 'flex', justifyContent: 'flex-end' }}>
                                <button onClick={handleClearCart}
                                    style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.72rem', letterSpacing: '1px', color: C.danger, background: 'none', border: 'none', cursor: 'pointer', textDecoration: 'underline' }}>
                                    Xóa tất cả
                                </button>
                            </div>
                        </div>

                        {/* Tổng đơn hàng */}
                        <div style={{ backgroundColor: C.white, border: `1px solid ${C.border}`, padding: '28px' }}>
                            <h3 style={{ fontSize: '1rem', fontWeight: 400, color: C.dark, marginBottom: '20px', paddingBottom: '14px', borderBottom: `1px solid ${C.border}` }}>
                                Tổng đơn hàng
                            </h3>
                            <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '10px' }}>
                                <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.82rem', color: C.muted }}>Tạm tính</span>
                                <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.82rem', color: C.dark }}>{total.toLocaleString('vi-VN')}₫</span>
                            </div>
                            <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '20px' }}>
                                <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.82rem', color: C.muted }}>Phí vận chuyển</span>
                                <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.82rem', color: C.gold }}>Miễn phí</span>
                            </div>
                            <div style={{ display: 'flex', justifyContent: 'space-between', paddingTop: '14px', borderTop: `1px solid ${C.border}`, marginBottom: '24px' }}>
                                <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.85rem', fontWeight: 500, color: C.dark }}>Tổng cộng</span>
                                <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '1.1rem', fontWeight: 500, color: C.gold }}>{total.toLocaleString('vi-VN')}₫</span>
                            </div>

                            <button onClick={() => navigate('/checkout')}
                                style={{ width: '100%', padding: '14px', backgroundColor: C.dark, color: '#fff', border: 'none', cursor: 'pointer', fontFamily: "'Jost',sans-serif", fontSize: '0.78rem', letterSpacing: '2px', fontWeight: 500, textTransform: 'uppercase', marginBottom: '10px' }}>
                                TIẾN HÀNH ĐẶT HÀNG
                            </button>
                            <Link to="/" style={{ display: 'block', textAlign: 'center', fontFamily: "'Jost',sans-serif", fontSize: '0.72rem', letterSpacing: '1px', color: C.muted, textDecoration: 'none', padding: '10px 0' }}>
                                ← Tiếp tục mua sắm
                            </Link>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
}
