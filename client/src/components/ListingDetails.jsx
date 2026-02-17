/* eslint-disable react/react-in-jsx-scope */
import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import { fetchListingById, clearCurrentListing } from '../store/slices/listingsSlice';
import axios from 'axios';
import { motion } from 'framer-motion';

export default function ListingDetails() {
    const { id } = useParams();
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { currentListing, loading, error } = useSelector((state) => state.listings);
    const { token } = useSelector((state) => state.auth);

    // Booking Form State
    const [startDate, setStartDate] = useState('');
    const [endDate, setEndDate] = useState('');
    const [bookingStatus, setBookingStatus] = useState(null); // 'processing', 'success', 'error'

    useEffect(() => {
        dispatch(fetchListingById(id));
        return () => dispatch(clearCurrentListing());
    }, [dispatch, id]);

    const handleBooking = async (e) => {
        e.preventDefault();
        if (!token) return navigate('/login');

        setBookingStatus('processing');
        try {
            await axios.post('/api/Bookings', {
                listingId: id,
                startDate,
                endDate
            }, {
                headers: { Authorization: `Bearer ${token}` }
            });
            setBookingStatus('success');
        } catch (err) {
            setBookingStatus('error');
            console.error(err);
        }
    };

    if (loading) return (
        <div className="d-flex justify-content-center mt-5">
            <div className="spinner-border text-primary" role="status">
                <span className="visually-hidden">Loading Details...</span>
            </div>
        </div>
    );
    if (error) return <div className="alert alert-danger m-4">Error loading listing.</div>;
    if (!currentListing) return null;

    return (
        <motion.div
            className="container mt-4"
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.5 }}
        >
            <button className="btn btn-outline-secondary mb-3" onClick={() => navigate(-1)}>
                &larr; Back to Listings
            </button>

            <div className="row">
                {/* 1. Listing Info */}
                <div className="col-md-8">
                    <img
                        src={`https://picsum.photos/seed/${currentListing.id}/800/600`}
                        className="img-fluid rounded mb-3 shadow-sm w-100"
                        alt="Listing"
                        style={{ objectFit: 'cover', maxHeight: '500px' }}
                    />

                    <div className="d-flex justify-content-between align-items-center">
                        <h1 className="display-5">{currentListing.title}</h1>
                        <span className="badge bg-primary fs-6">${currentListing.pricePerNight}/night</span>
                    </div>

                    <p className="lead text-muted"><i className="bi bi-geo-alt"></i> {currentListing.location}</p>

                    <hr />

                    <h4 className="mb-3">About this place</h4>
                    <p>{currentListing.description || "Experience comfort and style in this beautiful rental. Perfect for your next getaway."}</p>

                    <div className="row mt-4">
                        <div className="col-md-6">
                            <div className="card bg-light border-0 p-3">
                                <h5><i className="bi bi-door-closed"></i> Bedrooms</h5>
                                <p className="mb-0">{currentListing.bedrooms} Spacious Bedrooms</p>
                            </div>
                        </div>
                        <div className="col-md-6">
                            <div className="card bg-light border-0 p-3">
                                <h5><i className="bi bi-wifi"></i> Amenities</h5>
                                <p className="mb-0">Wifi, Kitchen, Workspace</p>
                            </div>
                        </div>
                    </div>
                </div>

                {/* 2. Booking Form (Fixed Side Panel) */}
                <div className="col-md-4">
                    <div className="card shadow border-0 p-4 sticky-top" style={{ top: '20px' }}>
                        <h4 className="text-primary mb-3">Book your stay</h4>

                        {bookingStatus === 'success' ? (
                            <div className="alert alert-success text-center">
                                <h5>🎉 Confirmed!</h5>
                                <p>Your booking has been placed.</p>
                                <button className="btn btn-success w-100" onClick={() => navigate('/')}>Explore More</button>
                            </div>
                        ) : (
                            <form onSubmit={handleBooking}>
                                <div className="mb-3">
                                    <label className="form-label fw-bold">Check-in</label>
                                    <input type="date" className="form-control" value={startDate} onChange={e => setStartDate(e.target.value)} required />
                                </div>
                                <div className="mb-3">
                                    <label className="form-label fw-bold">Check-out</label>
                                    <input type="date" className="form-control" value={endDate} onChange={e => setEndDate(e.target.value)} required />
                                </div>

                                <div className="d-grid">
                                    {bookingStatus === 'processing' ? (
                                        <button className="btn btn-secondary" disabled>
                                            <span className="spinner-border spinner-border-sm me-2"></span>
                                            Processing...
                                        </button>
                                    ) : (
                                        <button className="btn btn-lg btn-primary shadow-sm hover-scale">
                                            Reserve for ${currentListing.pricePerNight * 1}
                                        </button>
                                    )}
                                </div>
                                {bookingStatus === 'error' && <div className="text-danger mt-2 text-center">Booking Failed. Try different dates.</div>}
                            </form>
                        )}
                        <div className="text-muted text-center mt-3 small">
                            You won't be charged yet.
                        </div>
                    </div>
                </div>
            </div>
        </motion.div>
    );
}