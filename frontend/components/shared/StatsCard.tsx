import { LucideIcon } from 'lucide-react';
import { cn } from '@/lib/utils';

interface StatsCardProps {
  title: string;
  value: number | string;
  icon: LucideIcon;
  trend?: {
    value: number;
    direction: 'up' | 'down';
  };
  color?: 'blue' | 'green' | 'amber' | 'red' | 'purple';
  className?: string;
}

const colorStyles = {
  blue: 'bg-blue-500/10 text-blue-500',
  green: 'bg-green-500/10 text-green-500',
  amber: 'bg-amber-500/10 text-amber-500',
  red: 'bg-red-500/10 text-red-500',
  purple: 'bg-purple-500/10 text-purple-500',
};

export function StatsCard({
  title,
  value,
  icon: Icon,
  trend,
  color = 'blue',
  className,
}: StatsCardProps) {
  return (
    <div
      className={cn(
        'bg-zinc-900 border border-zinc-800 rounded-lg p-6',
        className
      )}
    >
      <div className="flex items-start justify-between">
        <div className="flex-1">
          <p className="text-sm font-medium text-zinc-400">{title}</p>
          <p className="text-3xl font-bold text-white mt-2">{value}</p>
          {trend && (
            <p className="text-sm mt-2">
              <span
                className={cn(
                  'font-medium',
                  trend.direction === 'up' ? 'text-green-500' : 'text-red-500'
                )}
              >
                {trend.direction === 'up' ? '↑' : '↓'} {trend.value}%
              </span>
              <span className="text-zinc-500 ml-1">vs. mês anterior</span>
            </p>
          )}
        </div>
        <div className={cn('p-3 rounded-lg', colorStyles[color])}>
          <Icon className="h-6 w-6" />
        </div>
      </div>
    </div>
  );
}
