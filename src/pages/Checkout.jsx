/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 09/06/2026
    Mô tả    : Trang Đặt Hàng (Checkout.jsx)
              - Form bắt buộc: FullName, Phone, Address
              - Đọc giỏ hàng từ localStorage
              - POST đơn hàng xuống Backend API
              - Gửi email xác nhận sau khi đặt thành công
              - Xóa giỏ hàng sau khi đặt thành công
*/
import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { getCart, saveCart } from './Cart';

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

const API_URL = process.env.REACT_APP_API_URL || 'https://localhost:7038/api';

export default function CheckoutPage() {
    const navigate = useNavigate();
    const [cartItems, setCartItems] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState(false);

    const [form, setForm] = useState({
        fullName: '',
        phone: '',
        address: '',
        notes: '',
    });

    const [errors, setErrors] = useState({});

    useEffect(() => {
        const cart = getCart();
        if (cart.length === 0) {
            navigate('/cart');
            return;
        }
        setCartItems(cart);

        // Tự động điền tên nếu đã đăng nhập
        const fullName = localStorage.getItem('fullName') || '';
        setForm(f => ({ ...f, fullName }));
    }, [navigate]);

    const total = cartItems.reduce((sum, i) => sum + i.price * i.quantity, 0);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setForm(f => ({ ...f, [name]: value }));
        if (errors[name]) setErrors(er => ({ ...er, [name]: '' }));
    };

    // Validate form
    const validate = () => {
        const newErrors = {};
        if (!form.fullName.trim()) newErrors.fullName = 'Vui lòng nhập họ tên.';
        if (!form.phone.trim()) {
            newErrors.phone = 'Vui lòng nhập số điện thoại.';
        } else if (!/^[0-9]{9,11}$/.test(form.phone.trim())) {
            newErrors.phone = 'Số điện thoại không hợp lệ (9-11 số).';
        }
        if (!form.address.trim()) newErrors.address = 'Vui lòng nhập địa chỉ giao hàng.';
        return newErrors;
    };

    const handleSubmit = async () => {
        const newErrors = validate();
        if (Object.keys(newErrors).length > 0) {
            setErrors(newErrors);
            return;
        }

        setLoading(true);
        setError('');

        try {
            const token = localStorage.getItem('token');

            const orderData = {
                fullName: form.fullName.trim(),
                phone: form.phone.trim(),
                address: form.address.trim(),
                notes: form.notes.trim(),
                items: cartItems.map(i => ({
                    productId: i.id,
                    productName: i.name,
                    quantity: i.quantity,
                    unitPrice: i.price,
                })),
                totalAmount: total,
            };

            const res = await fetch(`${API_URL}/orders`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    ...(token ? { Authorization: `Bearer ${token}` } : {}),
                },
                body: JSON.stringify(orderData),
            });

            if (!res.ok) {
                const msg = await res.text();
                throw new Error(msg || 'Đặt hàng thất bại. Vui lòng thử lại!');
            }

            // Xóa giỏ hàng sau khi đặt thành công
            saveCart([]);
            setSuccess(true);

        } catch (err) {
            setError(err.message || 'Có lỗi xảy ra. Vui lòng thử lại!');
        } finally {
            setLoading(false);
        }
    };

    // Màn hình thành công
    if (success) {
        return (
            <div style={{ backgroundColor: C.cream, minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', fontFamily: "'Cormorant Garamond',Georgia,serif" }}>
                <div style={{ backgroundColor: C.white, border: `1px solid ${C.border}`, padding: '56px 48px', textAlign: 'center', maxWidth: '480px', width: '100%' }}>
                    <div style={{ fontSize: '3rem', marginBottom: '20px' }}>✅</div>
                    <h2 style={{ fontSize: '1.6rem', fontWeight: 300, color: C.dark, marginBottom: '12px' }}>
                        Đặt hàng <em style={{ color: C.gold }}>thành công!</em>
                    </h2>
                    <p style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.85rem', color: C.muted, lineHeight: 1.7, marginBottom: '32px', fontWeight: 300 }}>
                        Cảm ơn bạn đã mua sắm tại Mai Trinh Studio.<br />
                        Email xác nhận đơn hàng đã được gửi về hộp thư của bạn.
                    </p>
                    <Link to="/" style={{ display: 'inline-block', fontFamily: "'Jost',sans-serif", fontSize: '0.78rem', letterSpacing: '2px', fontWeight: 500, textTransform: 'uppercase', padding: '13px 32px', border: `1px solid ${C.dark}`, color: C.dark, textDecoration: 'none' }}>
                        TIẾP TỤC MUA SẮM
                    </Link>
                </div>
            </div>
        );
    }

    const inputStyle = (field) => ({
        width: '100%',
        padding: '11px 14px',
        border: `1px solid ${errors[field] ? C.danger : C.border}`,
        backgroundColor: C.white,
        fontFamily: "'Jost',sans-serif",
        fontSize: '0.85rem',
        color: C.dark,
        outline: 'none',
        boxSizing: 'border-box',
    });

    const labelStyle = {
        fontFamily: "'Jost',sans-serif",
        fontSize: '0.72rem',
        letterSpacing: '1.5px',
        textTransform: 'uppercase',
        color: C.muted,
        fontWeight: 500,
        display: 'block',
        marginBottom: '6px',
    };

    return (
        <div style={{ backgroundColor: C.cream, minHeight: '100vh', fontFamily: "'Cormorant Garamond','Playfair Display',Georgia,serif" }}>
            <link href="https://fonts.googleapis.com/css2?family=Cormorant+Garamond:ital,wght@0,300;0,400;0,600;1,400&family=Jost:wght@300;400;500&display=swap" rel="stylesheet" />

            {/* Breadcrumb */}
            <div style={{ backgroundColor: C.white, borderBottom: `1px solid ${C.border}`, padding: '10px 40px' }}>
                <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.72rem', color: C.muted }}>
                    <Link to="/" style={{ color: C.muted, textDecoration: 'none' }}>🏠 Trang Chủ</Link>
                    <span style={{ margin: '0 8px', opacity: 0.4 }}>›</span>
                    <Link to="/cart" style={{ color: C.muted, textDecoration: 'none' }}>Giỏ Hàng</Link>
                    <span style={{ margin: '0 8px', opacity: 0.4 }}>›</span>
                    <span style={{ color: C.dark, fontWeight: 500 }}>Đặt Hàng</span>
                </span>
            </div>

            <div style={{ maxWidth: '1100px', margin: '0 auto', padding: '40px 24px' }}>
                <h1 style={{ fontSize: '1.8rem', fontWeight: 300, color: C.dark, marginBottom: '32px' }}>
                    Thông tin <em style={{ color: C.gold, fontStyle: 'italic' }}>đặt hàng</em>
                </h1>

                <div style={{ display: 'grid', gridTemplateColumns: '1fr 360px', gap: '24px', alignItems: 'start' }}>

                    {/* Form thông tin */}
                    <div style={{ backgroundColor: C.white, border: `1px solid ${C.border}`, padding: '32px' }}>
                        <h3 style={{ fontSize: '1rem', fontWeight: 400, color: C.dark, marginBottom: '24px', paddingBottom: '14px', borderBottom: `1px solid ${C.border}` }}>
                            Thông tin nhận hàng
                        </h3>

                        {/* Họ tên */}
                        <div style={{ marginBottom: '18px' }}>
                            <label style={labelStyle}>Họ và tên <span style={{ color: C.danger }}>*</span></label>
                            <input name="fullName" value={form.fullName} onChange={handleChange} placeholder="Nguyễn Văn A" style={inputStyle('fullName')} />
                            {errors.fullName && <p style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.72rem', color: C.danger, marginTop: '4px' }}>{errors.fullName}</p>}
                        </div>

                        {/* Số điện thoại */}
                        <div style={{ marginBottom: '18px' }}>
                            <label style={labelStyle}>Số điện thoại <span style={{ color: C.danger }}>*</span></label>
                            <input name="phone" value={form.phone} onChange={handleChange} placeholder="0912345678" style={inputStyle('phone')} />
                            {errors.phone && <p style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.72rem', color: C.danger, marginTop: '4px' }}>{errors.phone}</p>}
                        </div>

                        {/* Địa chỉ */}
                        <div style={{ marginBottom: '18px' }}>
                            <label style={labelStyle}>Địa chỉ giao hàng <span style={{ color: C.danger }}>*</span></label>
                            <input name="address" value={form.address} onChange={handleChange} placeholder="Số nhà, đường, phường/xã, quận/huyện, tỉnh/thành" style={inputStyle('address')} />
                            {errors.address && <p style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.72rem', color: C.danger, marginTop: '4px' }}>{errors.address}</p>}
                        </div>

                        {/* Ghi chú */}
                        <div style={{ marginBottom: '8px' }}>
                            <label style={labelStyle}>Ghi chú (tùy chọn)</label>
                            <textarea name="notes" value={form.notes} onChange={handleChange}
                                placeholder="Ghi chú thêm cho đơn hàng..."
                                rows={3}
                                style={{ ...inputStyle('notes'), resize: 'vertical' }} />
                        </div>
                    </div>

                    {/* Tóm tắt đơn hàng */}
                    <div>
                        <div style={{ backgroundColor: C.white, border: `1px solid ${C.border}`, padding: '28px', marginBottom: '16px' }}>
                            <h3 style={{ fontSize: '1rem', fontWeight: 400, color: C.dark, marginBottom: '20px', paddingBottom: '14px', borderBottom: `1px solid ${C.border}` }}>
                                Đơn hàng của bạn
                            </h3>

                            {cartItems.map(item => (
                                <div key={item.id} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '12px', gap: '10px' }}>
                                    <div style={{ display: 'flex', gap: '10px', alignItems: 'center', flex: 1 }}>
                                        <img src={item.imageUrl || 'https://images.unsplash.com/photo-1523381210434-271e8be1f52b?q=80&w=50'} alt={item.name}
                                            style={{ width: '44px', height: '44px', objectFit: 'cover', border: `1px solid ${C.border}`, flexShrink: 0 }} />
                                        <div>
                                            <p style={{ fontSize: '0.82rem', color: C.dark, margin: 0, lineHeight: 1.3 }}>{item.name}</p>
                                            <p style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.72rem', color: C.muted, margin: '2px 0 0 0' }}>x{item.quantity}</p>
                                        </div>
                                    </div>
                                    <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.82rem', color: C.dark, whiteSpace: 'nowrap' }}>
                                        {(item.price * item.quantity).toLocaleString('vi-VN')}₫
                                    </span>
                                </div>
                            ))}

                            <div style={{ borderTop: `1px solid ${C.border}`, paddingTop: '14px', marginTop: '14px' }}>
                                <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '8px' }}>
                                    <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.8rem', color: C.muted }}>Tạm tính</span>
                                    <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.8rem', color: C.dark }}>{total.toLocaleString('vi-VN')}₫</span>
                                </div>
                                <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '16px' }}>
                                    <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.8rem', color: C.muted }}>Phí vận chuyển</span>
                                    <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.8rem', color: C.gold }}>Miễn phí</span>
                                </div>
                                <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                                    <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '0.88rem', fontWeight: 500, color: C.dark }}>Tổng cộng</span>
                                    <span style={{ fontFamily: "'Jost',sans-serif", fontSize: '1.1rem', fontWeight: 500, color: C.gold }}>{total.toLocaleString('vi-VN')}₫</span>
                                </div>
                            </div>
                        </div>

                        {/* Lỗi */}
                        {error && (
                            <div style={{ backgroundColor: '#fff5f5', border: `1px solid ${C.danger}`, padding: '12px 16px', marginBottom: '14px', fontFamily: "'Jost',sans-serif", fontSize: '0.82rem', color: C.danger }}>
                                ⚠️ {error}
                            </div>
                        )}

                        {/* Nút đặt hàng */}
                        <button onClick={handleSubmit} disabled={loading}
                            style={{ width: '100%', padding: '15px', backgroundColor: loading ? C.muted : C.dark, color: '#fff', border: 'none', cursor: loading ? 'not-allowed' : 'pointer', fontFamily: "'Jost',sans-serif", fontSize: '0.78rem', letterSpacing: '2px', fontWeight: 500, textTransform: 'uppercase', marginBottom: '10px', transition: 'background 0.2s' }}>
                            {loading ? 'ĐANG XỬ LÝ...' : 'XÁC NHẬN ĐẶT HÀNG'}
                        </button>

                        <Link to="/cart" style={{ display: 'block', textAlign: 'center', fontFamily: "'Jost',sans-serif", fontSize: '0.72rem', letterSpacing: '1px', color: C.muted, textDecoration: 'none', padding: '10px 0' }}>
                            ← Quay lại giỏ hàng
                        </Link>
                    </div>
                </div>
            </div>
        </div>
    );
}
