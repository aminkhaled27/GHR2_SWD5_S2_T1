// Cart functionality
function showToast(title, message, type = 'success', actions = null) {
    console.log('Showing toast:', { title, message, type, actions });
    
    const toast = document.createElement('div');
    toast.className = 'custom-toast';
    toast.style.borderLeft = `4px solid ${type === 'success' ? '#28a745' : '#dc3545'}`;
    
    const header = document.createElement('div');
    header.className = 'toast-header';
    header.innerHTML = `
        <h6 class="toast-title">${title}</h6>
        <button type="button" class="btn-close" onclick="this.parentElement.parentElement.remove()"></button>
    `;
    
    const messageDiv = document.createElement('p');
    messageDiv.className = 'toast-message';
    messageDiv.textContent = message;
    
    toast.appendChild(header);
    toast.appendChild(messageDiv);
    
    if (actions) {
        const actionsDiv = document.createElement('div');
        actionsDiv.className = 'toast-actions';
        actions.forEach(action => {
            const button = document.createElement('button');
            button.className = `toast-btn toast-btn-${action.type}`;
            button.textContent = action.text;
            button.onclick = () => {
                action.handler();
                toast.remove();
            };
            actionsDiv.appendChild(button);
        });
        toast.appendChild(actionsDiv);
    }
    
    const container = document.querySelector('.toast-container');
    if (!container) {
        console.error('Toast container not found!');
        return;
    }
    
    container.appendChild(toast);
    console.log('Toast added to container');
    
    // Auto remove after 5 seconds if no actions
    if (!actions) {
        setTimeout(() => toast.remove(), 5000);
    }
}

// Function to update cart count
function updateCartCount(count) {
    console.log('Updating cart count to:', count);
    const cartCountElements = document.querySelectorAll('.cart-count');
    cartCountElements.forEach(element => {
        element.textContent = count;
    });
}

// Initialize cart functionality when document is ready
$(document).ready(function() {
    console.log('Document ready - initializing cart functionality');
    
    // Handle Add to Cart button clicks
    $('.btn-add-to-cart, .product-buy-btn').on('click', function(e) {
        console.log('Add to cart button clicked');
        e.preventDefault();
        e.stopPropagation();
        
        const button = $(this);
        const productId = button.data('productId');
        console.log('Product ID:', productId);
        
        if (!productId) {
            console.log('No product ID found');
            return;
        }
        
        // Show loading state
        const originalHtml = button.html();
        button.html('<i class="fas fa-spinner fa-spin"></i> Adding...');
        button.prop('disabled', true);
        
        // Make the AJAX call
        $.ajax({
            url: '/Cart/AddToCart',
            type: 'GET',
            data: { productId: productId },
            dataType: 'json',
            success: function(data) {
                console.log('Success:', data);
                if (data.success) {
                    // Update cart count
                    updateCartCount(data.cartCount);
                    
                    // Show success toast with actions
                    showToast(
                        'Success!',
                        'Product added to cart successfully.',
                        'success',
                        [
                            {
                                text: 'Continue Shopping',
                                type: 'secondary',
                                handler: () => {}
                            },
                            {
                                text: 'Go to Cart',
                                type: 'primary',
                                handler: () => window.location.href = '/Cart/ShowCart'
                            }
                        ]
                    );
                } else {
                    if (data.requiresLogin) {
                        window.location.href = '/Account/Login?returnUrl=' + encodeURIComponent(window.location.pathname);
                        return;
                    }
                    
                    showToast(
                        'Error',
                        data.message || 'An error occurred while adding the item to cart.',
                        'error'
                    );
                }
            },
            error: function(xhr, status, error) {
                console.error('Error:', { xhr, status, error });
                if (xhr.status === 401) {
                    window.location.href = '/Account/Login?returnUrl=' + encodeURIComponent(window.location.pathname);
                    return;
                }
                
                showToast(
                    'Error',
                    'An error occurred while adding the item to cart.',
                    'error'
                );
            },
            complete: function() {
                // Restore button state
                button.html(originalHtml);
                button.prop('disabled', false);
            }
        });
    });
}); 