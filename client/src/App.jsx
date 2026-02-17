/* eslint-disable react/react-in-jsx-scope */
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import ListingList from './components/ListingList';
import ListingDetails from './components/listingDetails';
import Login from './pages/Login';
import { useSelector, useDispatch } from 'react-redux';
import { logout } from './store/slices/authslice';
import useSignalR from './hooks/useSignalR.js';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

function App() {
  const { user } = useSelector((state) => state.auth);
  const dispatch = useDispatch();
  useSignalR();

  return (
    <BrowserRouter>
      {/* Navbar */}
      <nav className="navbar navbar-dark bg-dark mb-4 p-3">
        <div className="container">
          <Link className="navbar-brand" to="/">RentalMarket</Link>
          <div>
            {user ? (
               <button className="btn btn-outline-light btn-sm" onClick={() => dispatch(logout())}>Logout ({user})</button>
            ) : (
               <Link className="btn btn-outline-light btn-sm" to="/login">Login</Link>
            )}
          </div>
        </div>
      </nav>
      
      {/* Routes */}
      <Routes>
        <Route path="/" element={<ListingList />} />
        <Route path="/login" element={<Login />} />
        <Route path="/listing/:id" element={<ListingDetails />} /> {/* Coming soon */}
      </Routes>
    </BrowserRouter>
  );
}

export default App;