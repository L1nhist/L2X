namespace L2X.Data.Repositories;

/// <summary>
/// Repository cung cấp các phương thức xử lý dữ liệu chung cho các Bảng trong Database
/// </summary>
/// <typeparam name="TEnt">Phân lớp (type) của Entity tương ứng với Bảng cần xử lý</typeparam>
public interface IRepository<TEnt> : IDisposable
    where TEnt : class, IEntity
{
    IRepository<TEnt> IgnoreFilter();

    IRepository<TEnt> Query(Expression<Func<TEnt, bool>>? where = null, bool splitQuery = true, bool tracking = false);

    IRepository<TEnt> SortBy(params string[] flds);

    IRepository<TEnt> SortBy<TFld>(Expression<Func<TEnt, TFld>> selection, bool ascending = true);

    IRepository<TEnt> JoinBy(params string[] flds);

    IRepository<TEnt> JoinBy<TFld>(Expression<Func<TEnt, TFld>> selection);

    /// <summary>
    /// Thực thi tất cả những câu lệnh đang tồn tại vào Database
    /// </summary>
    Task<int> Commit();

    Task<int> Count();

    /// <inheritdoc/>
    Task<int> Min(Expression<Func<TEnt, int>> selector);

    /// <inheritdoc/>
    Task<long> Min(Expression<Func<TEnt, long>> selector);

    /// <inheritdoc/>
    Task<double> Min(Expression<Func<TEnt, double>> selector);

    /// <inheritdoc/>
    Task<decimal> Min(Expression<Func<TEnt, decimal>> selector);

    /// <inheritdoc/>
    Task<DateTime> Min(Expression<Func<TEnt, DateTime>> selector);

    /// <inheritdoc/>
    Task<int> Max(Expression<Func<TEnt, int>> selector);

    /// <inheritdoc/>
    Task<long> Max(Expression<Func<TEnt, long>> selector);

    /// <inheritdoc/>
    Task<double> Max(Expression<Func<TEnt, double>> selector);

    /// <inheritdoc/>
    Task<decimal> Max(Expression<Func<TEnt, decimal>> selector);

    /// <inheritdoc/>
    Task<DateTime> Max(Expression<Func<TEnt, DateTime>> selector);

    /// <inheritdoc/>
    Task<int> Sum(Expression<Func<TEnt, int>> selector);

    /// <inheritdoc/>
    Task<long> Sum(Expression<Func<TEnt, long>> selector);

    /// <inheritdoc/>
    Task<double> Sum(Expression<Func<TEnt, double>> selector);

    /// <inheritdoc/>
    Task<decimal> Sum(Expression<Func<TEnt, decimal>> selector);

    /// <summary>
    /// Thêm mới Entity vào Database
    /// </summary>
    /// <param name="ent">Giá trị Entity cần thêm</param>
    Task<int> Insert(TEnt ent);

    /// <summary>
    /// Thêm mới một danh sách các Entity vào Database
    /// </summary>
    /// <param name="ent">Danh sách các giá trị Entity cần thêm</param>
    Task<int> Insert(IEnumerable<TEnt> ents);

    /// <summary>
    /// Cập nhật Entity đã thay đổi vào Database
    /// </summary>
    /// <param name="ent">Giá trị Entity cần cập nhật</param>
    Task<int> Update(TEnt ent);

    /// <summary>
    /// Cập nhật một dánh sách các Entity đã thay đổi vào Database
    /// </summary>
    /// <param name="ent">Danh sách các giá trị Entity cần cập nhật</param>
    Task<int> Update(IEnumerable<TEnt> ents);

    /// <summary>
    /// Cập nhật một danh sách các Entity chỉ thay đổi các trường được liệt kê vào Database
    /// </summary>
    /// <param name="ent">Danh sách các giá trị Entity cần thay đổi</param>
    /// <param name="fields">Danh sách các trường dữ liệu cần thay đổi, được viết cách nhau bởi dấu phảy (,) và không phân biệt chữ hoa chữ thường</param>
    Task<int> UpdateBy(string field, object value);

    /// <summary>
    /// Cập nhật Entity chỉ thay đổi một trường dữ liệu vào Database
    /// </summary>
    /// <param name="ent">Giá trị Entity cần thay đổi</param>
    /// <param name="expression">Trường dữ liệu được chỉ ra trong công thức</param>
    Task<int> UpdateBy<TFld>(Expression<Func<TEnt, TFld>> expression, TFld value);

    /// <summary>
    /// Xóa một Entity khỏi Database
    /// </summary>
    /// <param name="ent">Giá trị Entity cần xóa</param>
    Task<int> Delete(bool firstOnly = false);

    /// <summary>
    /// Xóa một Entity khỏi Database
    /// </summary>
    /// <param name="ent">Giá trị Entity cần xóa</param>
    Task<int> Delete(TEnt ent);

    /// <summary>
    /// Xóa một Entity khỏi Database
    /// </summary>
    /// <param name="ent">Giá trị Entity cần xóa</param>
    Task<int> Delete(IEnumerable<TEnt> ents);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<int> DeleteAll();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    Task<TEnt?> GetByKeys(params object[] keys);

    /// <summary>
    /// Lấy một Entity theo một câu lệnh tìm kiếm có điều kiện từ Database
    /// </summary>
    /// <returns>Giá trị Entity tìm được</returns>
    Task<TEnt?> GetFirst();

    /// <summary>
    /// Lấy một Entity theo một câu lệnh tìm kiếm có điều kiện từ Database
    /// </summary>
    /// <returns>Giá trị Entity tìm được</returns>
    Task<TMap?> GetFirst<TMap>();

    /// <summary>
    /// Lấy danh sách phân trang các Entity theo một câu lệnh tìm kiếm có điều kiện từ Database
    /// </summary>
    /// <returns>Danh sách phân trang các Entity tìm được, <seealso cref="IEnumerable{TEnt}"/></returns>
    Task<List<TEnt>> GetList(int top = 0);

    /// <summary>
    /// Lấy danh sách phân trang các Entity theo một câu lệnh tìm kiếm có điều kiện từ Database
    /// </summary>
    /// <returns>Danh sách phân trang các Entity tìm được, <seealso cref="IEnumerable{TEnt}"/></returns>
    Task<List<TMap>> GetList<TMap>(int top = 0);

    /// <summary>
    /// Lấy danh sách phân trang các Entity theo một câu lệnh tìm kiếm có điều kiện từ Database
    /// </summary>
    /// <returns>Danh sách phân trang các Entity tìm được, <seealso cref="Pagination{TEnt}"/></returns>
    Task<Pagination<TEnt>> GetPaging(int page = 0, int size = 15);

    /// <summary>
    /// Lấy danh sách phân trang các Entity theo một câu lệnh tìm kiếm có điều kiện từ Database
    /// </summary>
    /// <returns>Danh sách phân trang các Entity tìm được, <seealso cref="Pagination{TEnt}"/></returns>
    Task<Pagination<TMap>> GetPaging<TMap>(int page = 0, int size = 15);
}