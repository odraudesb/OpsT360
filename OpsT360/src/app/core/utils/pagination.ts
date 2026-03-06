export const getResponsivePageSize = (): number => {
  if (typeof window === 'undefined') {
    return 10;
  }

  const width = window.innerWidth;
  if (width < 640) return 5;
  if (width < 1024) return 7;
  return 10;
};

export const clampPage = (page: number, totalPages: number): number =>
  Math.min(Math.max(page, 1), totalPages);
