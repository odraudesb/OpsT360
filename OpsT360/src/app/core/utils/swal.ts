type SwalOptions = {
  title: string;
  text?: string;
  confirmButtonText?: string;
  cancelButtonText?: string;
};

type SwalFireOptions = SwalOptions & {
  icon?: 'success' | 'error' | 'warning' | 'info' | 'question';
  showCancelButton?: boolean;
};

type SwalFireResult = {
  isConfirmed?: boolean;
};

type SwalFire = (options: SwalFireOptions) => Promise<SwalFireResult>;

const getSwalFire = (): SwalFire | null => {
  if (typeof window === 'undefined') return null;
  const swal = (window as Window & { Swal?: { fire?: SwalFire } }).Swal;
  return swal?.fire ?? null;
};

const swalError = async (options: SwalOptions): Promise<void> => {
  const fire = getSwalFire();
  if (!fire) return;
  await fire({ ...options, icon: 'error' });
};

const swalNotification = async (options: SwalOptions): Promise<void> => {
  const fire = getSwalFire();
  if (!fire) return;
  await fire({ ...options, icon: 'info' });
};

const swalOk = async (options: SwalOptions): Promise<void> => {
  const fire = getSwalFire();
  if (!fire) return;
  await fire({ ...options, icon: 'success' });
};

const swalQuestion = async (options: SwalOptions): Promise<boolean> => {
  const fire = getSwalFire();
  if (!fire) return false;
  const result = await fire({
    ...options,
    icon: 'question',
    showCancelButton: true,
    confirmButtonText: options.confirmButtonText ?? 'Confirm',
    cancelButtonText: options.cancelButtonText ?? 'Cancel',
  });
  return Boolean(result?.isConfirmed);
};

