# PriceTag

Price Tag is a C# WinForms (.NET 8) application that generates price labels with product name, price, discounted price, producer name, and either a classic barcode or a QR code **(license required)**.  
UI language: **Romanian**.

---

## Key features
- **Excel template** — Built-in template generator creates an Excel file with the required columns.
- **Optional toggles** — Discount price, barcode inclusion, and producer name can be turned on/off.  
  **If all three are off, the label contains only the product name and price.**
- **Discount options** — Loyalty-card or promotional discount with a custom percentage; labels reflect the chosen option.
- **Producer name (optional)** — If missing, you can proceed and the producer line is hidden for that item.
- **Barcode or QR** — Choose per label between classic barcode or QR code.
- **PDF export** — Labels are exported directly to PDF.

## Licensing & enrollment
The license is validated against your company/collection in the backend.  
On first launch, you sign in, request a one-time code delivered by email (valid for 24 hours), then confirm enrollment. If the account has a valid license, a **GUID** tied to your company is issued and stored locally in `PriceTag.xml` (in the application root).  
If the license is not valid, the app shows: **"Licenta actuala nu este valida! Contactati echipa de suport!"**

## Input & output
- **Input**: Excel created by the template generator (includes all required headers).
- **Output**: PDF with the rendered labels.

## Barcode & QR details
- **Barcode**: Code 39 (start/stop `*`, narrow/wide pattern). Rendered to bitmap and embedded in PDF.
- **QR code**: Generated with **QRCoder** (`ECCLevel.Q`); payload comes from your input (e.g., SKU/text/URL).

## Requirements
- Windows 10/11
- .NET 8 Runtime
- No external dependencies beyond those bundled with the app

## Install & run
1. Download and extract the release.
2. Run `PriceTag.exe`.
3. Complete enrollment on first launch.
4. Configure label options (discount, barcode/QR, producer name).
5. Generate the Excel template, fill it in, then process it to export PDF labels.

## Updates
- Manual distribution (no auto-update).

## Screenshots

<p align="center">
  <img src="docs/assets/Enrollment.png" alt="Enrollment screen" width="380">
</p>

<p align="center">
  <img src="docs/assets/PriceTag.png" alt="Main configuration screen" width="760">
</p>

### Output examples
<p align="center">
  <img src="docs/assets/BarcodeOutput.png" alt="Barcode label output" width="360">
  <img src="docs/assets/QRCodeOutput.png" alt="QR label output" width="360">
</p>
<p align="center">
  <img src="docs/assets/DiscountBarcodeOutput.png" alt="Discount + Barcode label output" width="360">
  <img src="docs/assets/DiscountQRCodeOutput.png" alt="Discount + QR label output" width="360">
</p>

## Notes
- `PriceTag.xml` (company GUID and basic settings) is saved in the **application root**.
