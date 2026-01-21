import { clsx, type ClassValue } from "clsx";
import { twMerge } from "tailwind-merge";

/**
 * Utility to merge Tailwind classes with proper precedence
 * Used by Shadcn/UI components
 */
export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}
