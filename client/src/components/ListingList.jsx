import { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { fetchListings } from '../store/slices/listingsSlice';
import { Link } from 'react-router-dom';
import SearchBar from './SearchBar';
import { motion } from 'framer-motion';

export default function ListingList() {
    const dispatch = useDispatch();
    const { items, loading, error } = useSelector((state) => state.listings);

    useEffect(() => {
        dispatch(fetchListings());
    }, [dispatch]);

    // Animation Variants
    const container = {
        hidden: { opacity: 0 },
        show: {
            opacity: 1,
            transition: { staggerChildren: 0.1 }
        }
    };

    const itemVariant = {
        hidden: { opacity: 0, y: 20 },
        show: { opacity: 1, y: 0 }
    };

    if (loading) return (
        <div className="d-flex justify-content-center mt-5">
            <div className="spinner-border text-primary" role="status">
                <span className="visually-hidden">Loading...</span>
            </div>
        </div>
    );

    if (error) return <div className="alert alert-danger m-4">{typeof error === 'object' ? JSON.stringify(error) : error}</div>;

    return (
        <div className="container mt-4">
            <div className="d-flex justify-content-between align-items-center mb-4">
                <h2 className="mb-0">Explore Rentals</h2>
            </div>

            <SearchBar />

            <motion.div
                className="row"
                variants={container}
                initial="hidden"
                animate="show"
            >
                {(!items || items.length === 0) ? (
                    <div className="col-12 text-center mt-4">
                        <p className="lead text-muted">No listings found matching your search.</p>
                    </div>
                ) : (
                    items.map(listing => (
                        <motion.div
                            key={listing.id}
                            className="col-md-4 mb-4"
                            variants={itemVariant}
                            whileHover={{ scale: 1.02 }}
                            whileTap={{ scale: 0.98 }}
                        >
                            <div className="card h-100 shadow-sm border-0">
                                <img
                                    src={`https://picsum.photos/seed/${listing.id}/600/400`}
                                    className="card-img-top"
                                    alt={listing.title}
                                    style={{ height: '200px', objectFit: 'cover' }}
                                />
                                <div className="card-body">
                                    <h5 className="card-title text-truncate" title={listing.title}>{listing.title}</h5>

                                    <div className="mb-2">
                                        <span className="badge bg-light text-dark border me-2">
                                            {// Using generic emoji/icon for now, bootstrapping icons might need import
                                            }
                                            📍 {listing.location}
                                        </span>
                                        <span className="badge bg-light text-dark border">
                                            🛏️ {listing.bedrooms} Beds
                                        </span>
                                    </div>

                                    <div className="d-flex justify-content-between align-items-end mt-3">
                                        <div>
                                            <span className="text-muted small">Price</span><br />
                                            <span className="h5 text-primary mb-0 fw-bold">${listing.pricePerNight}</span>
                                            <span className="text-muted small">/night</span>
                                        </div>
                                        <Link to={`/listing/${listing.id}`} className="btn btn-primary">
                                            Book Now
                                        </Link>
                                    </div>
                                </div>
                            </div>
                        </motion.div>
                    ))
                )}
            </motion.div>
        </div>
    );
}