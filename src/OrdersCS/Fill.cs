using System;
using System.Collections.Generic;
using System.Text;
using TradingEngineServer.Instrument;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Orders
{
    public class Fill
    {
        /// <summary>
        /// Initial processing time (in UTC) of the event that lead to the fill.
        /// </summary>
        public DateTime EventTime { get; set; }
        /// <summary>
        /// Order that was fully or partially filled.
        /// </summary>
        public IOrderCore OrderBase { get; set; }
        /// <summary>
        /// Whether the order filled was complete.
        /// </summary>
        public bool IsCompleteFill { get; set; }
        /// <summary>
        /// Quantity filled.
        /// </summary>
        public uint FillQuantity { get; set; }
        /// <summary>
        /// The Id of the fill, unique per fill.
        /// </summary>
        public long FillId { get; set; }
        /// <summary>
        /// Ties a pair of fills to a trade.
        /// </summary>
        public string ExecutionId { get; set; }
        /// <summary>
        /// The Id of the fill, unique per trade.
        /// </summary>
        public string FillExecutionId
        {
            get
            {
                return $"{ExecutionId}-{FillId}";
            }
        }
        /// <summary>
        /// Allocation algorithm used to fill the order with <see cref="OrderId"/>.
        /// </summary>
        public AllocationAlgorithm AllocationAlgorithm { get; set; }
    }
}
