import { ChevronLeft, ChevronRight } from 'lucide-react';

interface PaginationProps {
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
  hasNext: boolean;
  hasPrevious: boolean;
  totalCount: number;
}

export function Pagination({
  currentPage,
  totalPages,
  onPageChange,
  hasNext,
  hasPrevious,
  totalCount
}: PaginationProps) {
  // if (totalPages <= 1) return null; // Always show pagination to display counts

  return (
    <div className="flex items-center justify-between px-4 py-3 border-t border-zinc-800 sm:px-6">
      <div className="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
        <div>
          <p className="text-sm text-zinc-400">
            Mostrando <span className="font-medium text-white">{(currentPage - 1) * 10 + 1}</span> a <span className="font-medium text-white">{Math.min(currentPage * 10, totalCount)}</span> de{' '}
            <span className="font-medium text-white">{totalCount}</span> resultados
          </p>
        </div>
        <div>
          {totalPages > 1 && (
            <nav className="relative z-0 inline-flex rounded-md shadow-sm -space-x-px" aria-label="Pagination">
              <button
                onClick={() => onPageChange(currentPage - 1)}
                disabled={!hasPrevious}
                className={`relative inline-flex items-center px-2 py-2 rounded-l-md border border-zinc-700 bg-zinc-800 text-sm font-medium ${
                  !hasPrevious
                    ? 'text-zinc-500 cursor-not-allowed'
                    : 'text-zinc-300 hover:bg-zinc-700'
                }`}
              >
                <span className="sr-only">Anterior</span>
                <ChevronLeft className="h-5 w-5" aria-hidden="true" />
              </button>
              <button
                onClick={() => onPageChange(currentPage + 1)}
                disabled={!hasNext}
                className={`relative inline-flex items-center px-2 py-2 rounded-r-md border border-zinc-700 bg-zinc-800 text-sm font-medium ${
                  !hasNext
                    ? 'text-zinc-500 cursor-not-allowed'
                    : 'text-zinc-300 hover:bg-zinc-700'
                }`}
              >
                <span className="sr-only">Próxima</span>
                <ChevronRight className="h-5 w-5" aria-hidden="true" />
              </button>
            </nav>
          )}
        </div>
      </div>
      
      {/* Mobile View */}
      <div className="flex items-center justify-between w-full sm:hidden">
        <button
          onClick={() => onPageChange(currentPage - 1)}
          disabled={!hasPrevious}
          className={`relative inline-flex items-center px-4 py-2 border border-zinc-700 text-sm font-medium rounded-md ${
             !hasPrevious
              ? 'bg-zinc-900 text-zinc-500 cursor-not-allowed'
              : 'bg-zinc-800 text-white hover:bg-zinc-700'
          }`}
        >
          Anterior
        </button>
        <span className="text-sm text-zinc-400">
            {currentPage} / {totalPages}
        </span>
        <button
          onClick={() => onPageChange(currentPage + 1)}
          disabled={!hasNext}
          className={`ml-3 relative inline-flex items-center px-4 py-2 border border-zinc-700 text-sm font-medium rounded-md ${
            !hasNext
              ? 'bg-zinc-900 text-zinc-500 cursor-not-allowed'
              : 'bg-zinc-800 text-white hover:bg-zinc-700'
          }`}
        >
          Próxima
        </button>
      </div>
    </div>
  );
}
