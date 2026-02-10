export type Device = {
  deviceId: string;
  label?: string | null;
  location?: string | null;
  [key: string]: unknown;
};

export type TelemetryRequest = {
  deviceId?: string | null;
  eventId?: string | null;
  type?: string | null;
  value: number;
  unit?: string | null;
  recordedAt: string; // date-time
};

export type TelemetryResponse = {
  id: string; // uuid
  deviceId?: string | null;
  eventId?: string | null;
  type?: string | null;
  value: number;
  unit?: string | null;
  recordedAt: string; // date-time
};
