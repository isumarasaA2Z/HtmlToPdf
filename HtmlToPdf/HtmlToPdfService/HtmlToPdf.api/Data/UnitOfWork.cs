using HtmlToPdf.api.Data.Repositories;
using HtmlToPdf.core.Data;
using HtmlToPdf.core.Data.Entities;
using HtmlToPdf.core.Data.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace HtmlToPdf.api.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            PdfRepository = new PdfRepository(_context);
            AuditLogRepository = new AuditLogRepository(_context);
            PdfRequestRepository = new Repository<PdfGenerationRequest>(_context);
            TemplateRepository = new Repository<HtmlTemplate>(_context);
        }

        public IPdfRepository PdfRepository { get; private set; }
        public IAuditLogRepository AuditLogRepository { get; private set; }
        public IRepository<PdfGenerationRequest> PdfRequestRepository { get; private set; }
        public IRepository<HtmlTemplate> TemplateRepository { get; private set; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
