/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : BlogPage - Trang Tin Tức & Xu Hướng Thời Trang
               Đổi từ "Tin tức & Công nghệ" sang "Xu hướng & Phong cách thời trang"
*/
import React from 'react';
import PostList from '../components/PostList';

const BlogPage = () => {
    return (
        <div>
            {/* Banner tiêu đề trang */}
            <div
                className="bg-light py-4 border-bottom mb-4"
                style={{ borderLeft: '4px solid #ca1515' }}
            >
                <div className="container">
                    <h2 className="text-uppercase font-weight-bold mb-1" style={{ fontSize: '1.5rem' }}>
                        <i className="fa-solid fa-fire text-danger mr-2"></i>
                        Xu Hướng & Phong Cách
                    </h2>
                    <p className="text-muted mb-0 small">
                        Cập nhật xu hướng thời trang mới nhất, tips phối đồ và review sản phẩm
                    </p>
                </div>
            </div>

            {/* Danh sách bài viết */}
            <div className="container my-4">
                <PostList />
            </div>
        </div>
    );
};

export default BlogPage;
