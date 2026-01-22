'use client';

export function LoadingSpinner() {
  return (
    <div className="flex items-center justify-center min-h-screen">
      <div className="relative">
        {/* Spinner outer ring - Golden */}
        <div className="w-16 h-16 border-4 border-zinc-700 border-t-amber-500 rounded-full animate-spin"></div>
        
        {/* Inner pulse effect - Golden */}
        <div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2">
          <div className="w-8 h-8 bg-amber-500/20 rounded-full animate-pulse"></div>
        </div>
      </div>
    </div>
  );
}

// Variant for inline loading (smaller) - Golden
export function LoadingSpinnerInline() {
  return (
    <div className="flex items-center justify-center py-12">
      <div className="w-8 h-8 border-2 border-zinc-700 border-t-amber-500 rounded-full animate-spin"></div>
    </div>
  );
}
