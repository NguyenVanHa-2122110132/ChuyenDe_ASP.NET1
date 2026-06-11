/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Cập nhật BlogCategoryList - Sidebar chuyên mục tin tức thời trang
               Bẫy lỗi an toàn tránh lỗi parse "undefined" JSON
               Đổi từ chuyên mục công nghệ sang: Xu hướng, Tips phối đồ, Review sản phẩm...
*/
import React, { useState, useEffect } from 'react';
import blogService from '../services/blogService';

const BlogCategoryList = () => {
    const [blogCategories, setBlogCategories] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchBlogCategories = async () => {
            try {
                setLoading(true);
                const response = await blogService.getBlogCategories();
                // Kiểm tra nghiêm ngặt dữ liệu tránh lỗi "undefined" phá hỏng hệ thống
                if (response && (response.data || response)) {
                    setBlogCategories(response.data || response);
                }
            } catch (error) {
                console.error('Lỗi khi tải chuyên mục tin tức thời trang:', error);
            } finally {
                setLoading(false);
            }
        };
        fetchBlogCategories();
    }, []); // Mảng rỗng: chỉ gọi 1 lần khi component được mount

    if (loading) {
        return <div className="p-3 text-center text-muted small">🔄 Đang tải chuyên mục...</div>;
    }

    return (
        <div className="card shadow-sm border-0 rounded-lg mt-4">
            <div className="card-header bg-white border-bottom-0 pt-3 pb-2 px-4">
                <h6 className="card-title text-uppercase font-weight-bold text-secondary d-flex align-items-center mb-0">
                    {/* Icon tag phù hợp với blog thời trang */}
                    <i className="fa-solid fa-tags text-danger mr-2"></i> Chuyên Mục Tin Tức
                </h6>
            </div>
            <div className="card-body p-0">
                <div className="list-group list-group-flush">
                    {/* Kiểm tra nếu không phải mảng hoặc mảng rỗng */}
                    {!Array.isArray(blogCategories) || blogCategories.length === 0 ? (
                        <div className="p-3 text-center text-muted small">Chưa có chuyên mục nào.</div>
                    ) : (
                        blogCategories.map((item) => (
                            <div
                                key={item.id}
                                className="list-group-item d-flex justify-content-between align-items-center px-4 py-2 small text-secondary"
                                style={{ cursor: 'pointer', transition: 'background 0.2s' }}
                            >
                                <span># {item.name}</span>
                                <span className="badge badge-danger badge-pill">new</span>
                            </div>
                        ))
                    )}
                </div>
            </div>
        </div>
    );
};

export default BlogCategoryList;
