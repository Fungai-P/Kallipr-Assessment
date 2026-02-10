import { useEffect, useMemo, useState } from "react";
import { apiGet, apiPost } from "./api";
import type { Device, TelemetryRequest, TelemetryResponse } from "./types";
import "./app.css";

function isoNowUtc() {
  return new Date().toISOString();
}

function randomEventId() {
  return `evt-${Math.random().toString(16).slice(2, 8)}${Math.random().toString(16).slice(2, 6)}`;
}

export default function App() {
  const [customerIdInput, setCustomerIdInput] = useState<string>(() => localStorage.getItem("customerId") ?? "");
  const [customerId, setCustomerId] = useState<string>(() => localStorage.getItem("customerId") ?? "");

  const [devices, setDevices] = useState<Device[]>([]);
  const [selectedDeviceId, setSelectedDeviceId] = useState<string>("");

  const [telemetry, setTelemetry] = useState<TelemetryResponse[]>([]);
  const [loadingDevices, setLoadingDevices] = useState(false);
  const [loadingTelemetry, setLoadingTelemetry] = useState(false);

  const [posting, setPosting] = useState(false);
  const [postResult, setPostResult] = useState<string | null>(null);

  const [error, setError] = useState<string | null>(null);
  
  const [newEventId, setNewEventId] = useState<string>(() => randomEventId());
  const [newType, setNewType] = useState<string>("Temperature");
  const [newValue, setNewValue] = useState<string>("21.5");
  const [newUnit, setNewUnit] = useState<string>("Celcius");
  const [newRecordedAt, setNewRecordedAt] = useState<string>(isoNowUtc());

  async function loadDevices(tenant: string) {
    const data = await apiGet<Device[]>("/api/devices", tenant);
    data.sort((a, b) => (a.deviceId ?? "").localeCompare(b.deviceId ?? ""));
    setDevices(data);

    if (data.length > 0) setSelectedDeviceId(data[0].deviceId);
    else setSelectedDeviceId("");
  }

  async function loadTelemetry(tenant: string, deviceId: string) {
    const data = await apiGet<TelemetryResponse[]>(
      `/api/devices/${encodeURIComponent(deviceId)}/telemetry`,
      tenant
    );
    data.sort((a, b) => new Date(a.recordedAt).getTime() - new Date(b.recordedAt).getTime());
    setTelemetry(data);
  }

  // Load devices when tenant changes
  useEffect(() => {
    if (!customerId.trim()) {
      setDevices([]);
      setSelectedDeviceId("");
      setTelemetry([]);
      return;
    }

    (async () => {
      try {
        setError(null);
        setPostResult(null);
        setLoadingDevices(true);
        setDevices([]);
        setSelectedDeviceId("");
        setTelemetry([]);
        await loadDevices(customerId.trim());
      } catch (e: any) {
        setError(e?.message ?? "Failed to load devices");
      } finally {
        setLoadingDevices(false);
      }
    })();
  }, [customerId]);
  
  useEffect(() => {
    if (!customerId.trim() || !selectedDeviceId) {
      setTelemetry([]);
      return;
    }

    (async () => {
      try {
        setError(null);
        setPostResult(null);
        setLoadingTelemetry(true);
        await loadTelemetry(customerId.trim(), selectedDeviceId);
      } catch (e: any) {
        setError(e?.message ?? "Failed to load telemetry");
      } finally {
        setLoadingTelemetry(false);
      }
    })();
  }, [customerId, selectedDeviceId]);

  const selectedDevice = useMemo(
    () => devices.find((d) => d.deviceId === selectedDeviceId) ?? null,
    [devices, selectedDeviceId]
  );

  const insights = useMemo(() => {
    if (!telemetry.length) return null;

    const values = telemetry.map((t) => t.value);
    const min = Math.min(...values);
    const max = Math.max(...values);
    const avg = values.reduce((a, b) => a + b, 0) / values.length;

    const latest = telemetry.reduce((acc, cur) =>
      new Date(cur.recordedAt).getTime() > new Date(acc.recordedAt).getTime() ? cur : acc
    );

    return { min, max, avg, latest };
  }, [telemetry]);

  function applyCustomer() {
    const id = customerIdInput.trim();
    setCustomerId(id);
    localStorage.setItem("customerId", id);
  }

  function clearCustomer() {
    setCustomerIdInput("");
    setCustomerId("");
    localStorage.removeItem("customerId");
  }

  async function submitTelemetry() {
    setPostResult(null);

    const tenant = customerId.trim();
    if (!tenant) {
      setError("Please apply a customer id first.");
      return;
    }
    if (!selectedDeviceId) {
      setError("Please select a device first.");
      return;
    }

    const parsed = Number(newValue);
    if (Number.isNaN(parsed)) {
      setError("Value must be a number.");
      return;
    }

    const recordedAtIso = new Date(newRecordedAt).toISOString();

    const payload: TelemetryRequest = {
      deviceId: selectedDeviceId,
      eventId: newEventId.trim() || null,
      type: newType.trim() || null,
      value: parsed,
      unit: newUnit.trim() || null,
      recordedAt: recordedAtIso
    };

    try {
      setError(null);
      setPosting(true);

      const resp = await apiPost<TelemetryRequest, TelemetryResponse>("/api/telemetry", tenant, payload);
      setPostResult(`Created: ${resp.id}`);

      await loadTelemetry(tenant, selectedDeviceId);

      setNewEventId(randomEventId());
      setNewRecordedAt(isoNowUtc());
    } catch (e: any) {
      setError(e?.message ?? "Failed to create telemetry");
    } finally {
      setPosting(false);
    }
  }

  return (
    <div className="page">
      <header className="header">
        <div>
          <h1 className="h1">Device Telemetry UI</h1>
          <div className="muted">
            Sends tenant header <code>X-Customer-Id</code> on all API calls.
          </div>
        </div>

        <div className="tenantBox">
          <label className="label">
            Customer ID
            <input
              className="input"
              value={customerIdInput}
              onChange={(e) => setCustomerIdInput(e.target.value)}
              placeholder='e.g. "acme-123"'
            />
          </label>

          <div className="actions">
            <button className="btn" onClick={applyCustomer} disabled={!customerIdInput.trim()}>
              Apply
            </button>
            <button className="btn secondary" onClick={clearCustomer}>
              Clear
            </button>
          </div>

          <div className="mutedSmall">
            API base: <code>{import.meta.env.VITE_API_BASE ?? "http://localhost:7073"}</code>
          </div>
        </div>
      </header>

      {error && <div className="alert">{error}</div>}
      {postResult && <div className="success">{postResult}</div>}

      <main className="grid">
        <section className="card">
          <div className="cardTitle">Devices (GET /api/devices)</div>

          {!customerId.trim() ? (
            <div className="muted">Enter a customer ID and click Apply.</div>
          ) : loadingDevices ? (
            <div className="muted">Loading devices…</div>
          ) : devices.length === 0 ? (
            <div className="muted">No devices returned for this tenant.</div>
          ) : (
            <>
              <label className="label">
                Device
                <select
                  className="select"
                  value={selectedDeviceId}
                  onChange={(e) => setSelectedDeviceId(e.target.value)}
                >
                  {devices.map((d) => (
                    <option key={d.deviceId} value={d.deviceId}>
                      {d.deviceId}
                      {d.label ? ` — ${d.label}` : ""}
                      {d.location ? ` (${d.location})` : ""}
                    </option>
                  ))}
                </select>
              </label>

              {selectedDevice && (
                <div className="deviceMeta">
                  <div>
                    <b>DeviceId:</b> {selectedDevice.deviceId}
                  </div>
                  {selectedDevice.label ? (
                    <div>
                      <b>Label:</b> {String(selectedDevice.label)}
                    </div>
                  ) : null}
                  {selectedDevice.location ? (
                    <div>
                      <b>Location:</b> {String(selectedDevice.location)}
                    </div>
                  ) : null}
                </div>
              )}

              <div className="divider" />

              <div className="cardTitle">Create telemetry (POST /api/telemetry)</div>

              <div className="formGrid">
                <label className="label">
                  EventId
                  <input
                    className="input"
                    value={newEventId}
                    onChange={(e) => setNewEventId(e.target.value)}
                  />
                </label>

                <label className="label">
                  Type
                  <input className="input" value={newType} onChange={(e) => setNewType(e.target.value)} />
                </label>

                <label className="label">
                  Value
                  <input
                    className="input"
                    value={newValue}
                    onChange={(e) => setNewValue(e.target.value)}
                    inputMode="decimal"
                  />
                </label>

                <label className="label">
                  Unit
                  <input className="input" value={newUnit} onChange={(e) => setNewUnit(e.target.value)} />
                </label>

                <label className="label span2">
                  RecordedAt (UTC)
                  <input
                    className="input"
                    value={newRecordedAt}
                    onChange={(e) => setNewRecordedAt(e.target.value)}
                    placeholder="ISO string or any date value"
                  />
                  <div className="hint">Example: 2025-05-04T12:34:56Z</div>
                </label>

                <div className="span2">
                  <button className="btn btnFull" onClick={submitTelemetry} disabled={posting || !selectedDeviceId}>
                    {posting ? "Posting…" : "Create event"}
                  </button>
                </div>
              </div>
            </>
          )}
        </section>

        <section className="card">
          <div className="cardTitle">Telemetry (GET /api/devices/{`{deviceId}`}/telemetry)</div>

          {!customerId.trim() ? (
            <div className="muted">Apply a customer ID first.</div>
          ) : !selectedDeviceId ? (
            <div className="muted">Select a device.</div>
          ) : loadingTelemetry ? (
            <div className="muted">Loading telemetry…</div>
          ) : (
            <>
              {insights ? (
                <div className="insights">
                  <div className="insight">
                    <div className="k">Latest</div>
                    <div className="v">
                      {insights.latest.value} {insights.latest.unit ?? ""}
                    </div>
                    <div className="s">{new Date(insights.latest.recordedAt).toISOString()}</div>
                  </div>

                  <div className="insight">
                    <div className="k">Min / Avg / Max</div>
                    <div className="v">
                      {insights.min} / {insights.avg.toFixed(2)} / {insights.max}
                    </div>
                    <div className="s">{telemetry.length} points</div>
                  </div>
                </div>
              ) : (
                <div className="muted">No telemetry events for this device.</div>
              )}

              {telemetry.length > 0 && (
                <div className="tableWrap">
                  <table className="table">
                    <thead>
                      <tr>
                        <th>RecordedAt (UTC)</th>
                        <th>Type</th>
                        <th>Value</th>
                        <th>Unit</th>
                        <th>EventId</th>
                        <th>Id</th>
                      </tr>
                    </thead>
                    <tbody>
                      {telemetry.map((t) => (
                        <tr key={t.id}>
                          <td>{new Date(t.recordedAt).toISOString()}</td>
                          <td>{t.type ?? ""}</td>
                          <td>{t.value}</td>
                          <td>{t.unit ?? ""}</td>
                          <td>{t.eventId ?? ""}</td>
                          <td className="mono">{t.id}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </>
          )}
        </section>
      </main>
    </div>
  );
}
