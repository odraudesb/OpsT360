import type { Field } from './field.type';

const MAX_DEPTH = 6;

export const buildXmlDetailsFields = (xmlDetails: unknown): Field[] => {
  const xml = normalizeXmlDetails(xmlDetails);
  const cont = pickCaseInsensitive(xml, 'Contenedor');
  const xmlToShow = isRecord(cont) ? cont : xml;

  if (!isRecord(xmlToShow) || Object.keys(xmlToShow).length === 0) {
    return [{ label: '—', value: '—' }];
  }

  const out: Field[] = [];
  for (const key of Object.keys(xmlToShow)) {
    if (isIdLikeKey(key)) continue;
    out.push(...valueToFields((xmlToShow as Record<string, unknown>)[key], key));
  }
  return compactFields(out);
};

const normalizeKey = (s: string): string => (s ?? '').toString().trim().toLowerCase();

const isIdLikeKey = (key: string): boolean => normalizeKey(key).includes('id');

const normalizeXmlDetails = (xmlDetails: unknown): Record<string, unknown> => {
  if (!xmlDetails) return {};
  if (typeof xmlDetails === 'string') {
    try {
      const parsed = JSON.parse(xmlDetails) as unknown;
      return isRecord(parsed) ? parsed : {};
    } catch {
      return parseXmlDetails(xmlDetails);
    }
  }
  return isRecord(xmlDetails) ? xmlDetails : {};
};

const pickCaseInsensitive = (obj: Record<string, unknown>, key: string): unknown => {
  if (key in obj) return obj[key];
  const found = Object.keys(obj).find((k) => k.toLowerCase() === key.toLowerCase());
  return found ? obj[found] : undefined;
};

const valueToFields = (value: unknown, label: string, depth = 0): Field[] => {
  if (value == null) return [{ label: formatLabel(label), value: '—' }];
  if (depth > MAX_DEPTH) {
    return [{ label: formatLabel(label), value: safeString(value) }];
  }

  if (typeof value === 'string' || typeof value === 'number' || typeof value === 'boolean') {
    return [{ label: formatLabel(label), value: String(value) }];
  }

  if (Array.isArray(value)) {
    if (!value.length) {
      return [{ label: formatLabel(label), value: '—' }];
    }
    const nested: Field[] = [];
    value.forEach((item, index) => {
      nested.push(...valueToFields(item, `${label} / ${index + 1}`, depth + 1));
    });
    return nested.length ? nested : [{ label: formatLabel(label), value: `${value.length} item(s)` }];
  }

  if (isRecord(value)) {
    const nested: Field[] = [];
    for (const key of Object.keys(value)) {
      if (isIdLikeKey(key)) continue;
      nested.push(...valueToFields((value as Record<string, unknown>)[key], `${label} / ${key}`, depth + 1));
    }
    return nested.length ? nested : [{ label: formatLabel(label), value: '—' }];
  }

  return [{ label: formatLabel(label), value: safeString(value) }];
};

const formatLabel = (label: string): string => {
  return (
    String(label ?? '')
      .split('/')
      .map((part) => part.trim().replace(/_/g, ' '))
      .filter(Boolean)
      .join(' / ') || '—'
  );
};

const compactFields = (fields: Field[]): Field[] => {
  const cleaned = fields
    .map((f) => ({
      ...f,
      label: (f.label ?? '').toString().trim() || '—',
      value: (f.value ?? '').toString().trim() || '—',
    }))
    .filter((f) => f.label !== '' && !isIdLikeKey(f.label));

  const seen = new Set<string>();
  const unique: Field[] = [];
  for (const f of cleaned) {
    const key = f.label.toLowerCase();
    if (seen.has(key)) continue;
    seen.add(key);
    unique.push(f);
  }
  return unique;
};

const safeString = (value: unknown): string => {
  try {
    return JSON.stringify(value);
  } catch {
    return String(value);
  }
};

const isRecord = (value: unknown): value is Record<string, unknown> =>
  !!value && typeof value === 'object' && !Array.isArray(value);

const parseXmlDetails = (xmlString: string): Record<string, unknown> => {
  const trimmed = xmlString.trim();
  if (!trimmed.startsWith('<')) return {};

  try {
    const parser = new DOMParser();
    const doc = parser.parseFromString(trimmed, 'text/xml');
    if (doc.getElementsByTagName('parsererror').length > 0) {
      return {};
    }
    const root = doc.documentElement;
    if (!root) return {};
    const rootValue = xmlElementToObject(root);
    return { [root.tagName]: rootValue };
  } catch {
    return {};
  }
};

const xmlElementToObject = (node: Element): Record<string, unknown> | string => {
  const children = Array.from(node.children);
  if (!children.length) {
    return (node.textContent ?? '').trim();
  }
  const result: Record<string, unknown> = {};
  children.forEach((child) => {
    const key = child.tagName;
    const value = xmlElementToObject(child);
    if (key in result) {
      const existing = result[key];
      if (Array.isArray(existing)) {
        existing.push(value);
      } else {
        result[key] = [existing, value];
      }
    } else {
      result[key] = value;
    }
  });
  return result;
};
