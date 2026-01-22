'use client';

import { MoreVertical } from 'lucide-react';
import { useState, useRef, useEffect } from 'react';
import { cn } from '@/lib/utils';

export interface ActionMenuItem {
  label: string;
  onClick: () => void;
  icon?: React.ReactNode;
  variant?: 'default' | 'success' | 'warning' | 'danger';
  type?: 'item' | 'divider';
}

interface ActionMenuProps {
  items: ActionMenuItem[];
}

export function ActionMenu({ items }: ActionMenuProps) {
  const [isOpen, setIsOpen] = useState(false);
  const menuRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (menuRef.current && !menuRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    }

    if (isOpen) {
      document.addEventListener('mousedown', handleClickOutside);
    }

    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, [isOpen]);

  const variantStyles = {
    default: 'text-zinc-300 hover:text-white hover:bg-zinc-800',
    success: 'text-green-400 hover:text-green-300 hover:bg-green-500/10',
    warning: 'text-amber-400 hover:text-amber-300 hover:bg-amber-500/10',
    danger: 'text-red-400 hover:text-red-300 hover:bg-red-500/10',
  };

  return (
    <div className="relative" ref={menuRef}>
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="p-2 hover:bg-zinc-800 rounded-lg transition-colors"
      >
        <MoreVertical className="h-4 w-4 text-zinc-400" />
      </button>

      {isOpen && (
        <div className="absolute right-0 mt-2 w-48 bg-zinc-900 border border-zinc-800 rounded-lg shadow-lg z-10">
          <div className="py-1">
            {items.map((item, index) => {
              if (item.type === 'divider') {
                return (
                  <div
                    key={index}
                    className="my-1 border-t border-zinc-800"
                  />
                );
              }

              return (
                <button
                  key={index}
                  onClick={() => {
                    item.onClick();
                    setIsOpen(false);
                  }}
                  className={cn(
                    'w-full flex items-center gap-2 px-4 py-2 text-sm transition-colors',
                    variantStyles[item.variant || 'default']
                  )}
                >
                  {item.icon && <span className="h-4 w-4">{item.icon}</span>}
                  <span>{item.label}</span>
                </button>
              );
            })}
          </div>
        </div>
      )}
    </div>
  );
}
