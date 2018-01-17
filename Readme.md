# Readme for Final Examine
# Quick Note

### ---------------- INFO ----------------
1. ID: 1512029
2. Name: Trần Quốc Bảo
3. Emal: tranquocbao3897@gmail.com
4. Version Visual Studio: Community 2017.
5. Release: Bản release được build ở dạng release.
6. Application: Chương trình được viết bằng C# (WPF).

### -------------- FEATURES --------------
1. Khi khởi động chương trình hiển thị một notify icon trong phần notification area,
nhấp chuột trái sẽ hiển thị các note và các tag, nhấp chuột phải hiển thị context menu
cho người dùng chọn, New note, View notes, View statistics, Exit.
2. Sử dụng phím tắt để tạo một note mới. (Ctrl + Space)
3. Giao diện thêm note mới trực quan. Bao gồm:

- Có hướng dẫn ở mỗi control bằng chữ hoặc ToolTip.
- Nội dung note có thể Undo, Redo bằng phím tắt:
Ctrl + Z: Undo
Ctrl + Y: Redo
- Nội dung note có thể định dạng được bằng phím tắt:
Ctrl + B: In đậm các chữ đang được chọn
Ctrl + I: In nghiêng các chữ đang được chọn
Ctrl + U: Gạch chân các chứ đang được chọn
Ctrl + L, E, R: Căn lề cho một đoạn văn (L:Left, E:Center, R:Right), đoạn văn bản là các đoạn cách nhau cách xuống dòng (Enter)
- Thêm tag, có thể search tag, không có tag thì có thể tạo tag mới.
- Có button setting để chỉnh màu nền của note.
- Có thể resize của sổ, cũng như di chuyển.

4. Quản lý các note và tag. Giao diện gồm một ListView chứa các note và TreeView chứa các tag,
khi người dùng bấm vào các tag sẽ hiển thị các note tương ứng của tag đó.
5. ListView chứa các note có các chức năng nâng cao:

- Sắp xếp dữ liệu của một cột khi người dùng bấm vào header của cột đó,
có hiển thị dấu mũi tên để biết khi nào đang xếp tăng và khi nào là đang xếp giảm.
- Khi nhấp chuột phải vào một item note trong danh sách,
hiện ra context menu cho phép người dùng chọn xóa item, chỉnh sửa item, hoặc hiển thị item ra một cửa sổ note.

6. TreeView chứa các tag có các chức năng nâng cao:

- Thứ tự các tag trong TreeView được sắp xếp tăng dần theo tên tag.
- Khi nhấp chuột phải vào một item tag trong danh sách,
hiện ra context menu cho phép người dùng chọn xóa item, hoặc chỉnh sửa tên item.

7. Thống kê vẽ biểu đồ tròn các tag có trong chương trình dựa theo số lượng note của tag đó,
trên biểu đồ tròn khi để chuột vào một phần trong biểu đồ thì sẽ hiện ra thông tin của phần đó (ToolTip).
8. Hệ thống sao lưu dữ liệu tự động vào trong một file database của sqlite,
chương trình có sử dụng một package nuget là [SQLitePCL](https://www.nuget.org/packages/SQLitePCL) (Portable Class Library for SQLite)
9. Truy cập dữ liệu dùng các thao tác trên các bảng tương ứng. (SQL)
10. Sao lưu note cũng dễ dàng hơn chỉ việc copy file database và đem sử dụng ở nơi khác.

### ------------- MAIN FLOW --------------
1. Khởi động chương trình, chương trình sẽ tự động tạo database nếu chưa có.
2. Sử dụng phím tắt (Ctrl + Space) hoặc, nhấp chuột phải vào notify icon chọn new note,
để hiển thị cửa sổ thêm một note mới.
3. Điền các thông tin về một note, title, content, add tag vào note. Chọn "Xong" để lưu note.
4. Nhấp chuột phải vào notify icon chọn view notes để mở cửa sổ quản lý note. 
Ở cửa sổ quản lý note ta có thể xóa, sửa, hiển thị note - xóa, sửa tag.
5. Thống kê, nhấp chuột phải vào notify icon chọn view statistics để xem thống kê tag với số note tương ứng.
6. Thoát chương trình, nhấp chuột phải vào notify icon chọn exit để thoát.

### ---------- ADDITIONAL FLOWS ----------
1. Không cho tạo note với content rỗng.
2. Nếu note không có tag thì sẽ tự tạo một tag có tên là Other để chứa.
3. Khi xóa tag nếu có note chỉ thuộc về tag đó sẽ tự động chuyển qua tag Other để chứa.
4. Các tag có số lượng note là 0 vẫn không bị xóa khỏi database.
5. Khi thoát cửa sổ view notes sẽ thoát luôn chương trình,
khi minimize cửa sổ view notes sẽ tiếp tục chạy chương trình với notify icon.

### ------------- ATTENTION --------------
1. Chương trình được build Bản VS Community 2017 ver 15.4.4
(Có thể phải retarget về phiên bản thấp hơn)
2. Chương trình được viết bằng C# (WPF)

### ------------ LINK BITBUCKET -----------
> https://1512029@bitbucket.org/1512029/final.quicknote.git
```
Chú ý link repository trên là private đã add key public của giáo viên.
Đồng thời add user tdquang_edu vào repository với quyền read.
```
### ------------- LINK YOUTUBE ------------
> https://youtu.be/xBgTd5KrtsM