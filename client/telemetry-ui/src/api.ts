export const API_BASE = import.meta.env.VITE_API_BASE ?? "https://localhost:7073";

async function readError(res: Response) {
  const text = await res.text();
  return text || `HTTP ${res.status}`;
}

export async function apiGet<T>(path: string, customerId: string): Promise<T> {
  const res = await fetch(`${API_BASE}${path}`, {
    method: "GET",
    headers: {
      "X-Customer-Id": customerId
    }
  });

  if (!res.ok) throw new Error(await readError(res));
  return (await res.json()) as T;
}

export async function apiPost<TReq, TRes>(path: string, customerId: string, body: TReq): Promise<TRes> {
  const res = await fetch(`${API_BASE}${path}`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      "X-Customer-Id": customerId
    },
    body: JSON.stringify(body)
  });

  if (!res.ok) throw new Error(await readError(res));
  return (await res.json()) as TRes;
}
