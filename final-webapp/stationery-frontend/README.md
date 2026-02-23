# Stationery Store Frontend

A modern React + TypeScript frontend for the Stationery Store API.

## Features

- 🔐 JWT Authentication with login/logout
- 📊 Dashboard with statistics
- 📦 Products management
- 🛒 Orders management
- 👥 Customers management
- 👤 Users management
- 🌐 Bilingual support (English/Arabic)
- 📱 Responsive design
- 🎨 Bootstrap 5 styling

## Tech Stack

- React 19
- TypeScript
- Vite
- React Router DOM
- Axios
- TanStack Query (React Query)
- React Hook Form
- Bootstrap 5
- Bootstrap Icons

## Getting Started

### Prerequisites

- Node.js 18+ 
- npm or yarn

### Installation

```bash
cd stationery-frontend
npm install
```

### Development

```bash
npm run dev
```

The app will be available at `http://localhost:3000`

### Build

```bash
npm run build
```

### Preview Production Build

```bash
npm run preview
```

## API Configuration

The frontend is configured to proxy API requests to the backend at `http://localhost:8080`.

Update `vite.config.ts` if your backend is running on a different port:

```typescript
server: {
  proxy: {
    '/api': {
      target: 'http://localhost:8080',
      changeOrigin: true,
    },
  },
}
```

## Default Login

- Username: `admin`
- Password: `password123`

## Project Structure

```
src/
├── components/
│   └── Layout.tsx          # Main layout with sidebar
├── context/
│   └── AuthContext.tsx     # Authentication context
├── pages/
│   ├── Login.tsx           # Login page
│   ├── Dashboard.tsx       # Dashboard
│   ├── Products.tsx        # Products list
│   ├── Orders.tsx          # Orders list
│   ├── Customers.tsx       # Customers list
│   └── Users.tsx           # Users list
├── services/
│   └── api.ts              # API service
├── types/
│   └── index.ts            # TypeScript types
├── App.tsx                 # Main app component
├── main.tsx                # Entry point
└── index.css               # Global styles
```

## License

MIT
